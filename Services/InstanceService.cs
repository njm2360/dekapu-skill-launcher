using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using DekapuSkillLauncher.Models;

namespace DekapuSkillLauncher.Services;

public class InstanceService(AppSettings settings)
{
    private static readonly HttpClient _client = CreateClient();
    private readonly Dictionary<string, InstanceCache> _caches = [];

    private static HttpClient CreateClient()
    {
        var name = Assembly.GetExecutingAssembly().GetName();
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
            name.Name!, $"{name.Version?.Major}.{name.Version?.Minor}.{name.Version?.Build}"));
        return client;
    }

    public async Task<InstanceCache> GetInstancesAsync(string groupId, bool refresh = false, CancellationToken cancellationToken = default)
    {
        if (!refresh && _caches.TryGetValue(groupId, out var cached))
            return cached;

        var cache = await HttpRetryHelper.ExecuteAsync(async ct =>
        {
            var json = await _client.GetStringAsync($"{settings.ApiBaseUrl}/instances/{groupId}", ct);
            var arr = JsonSerializer.Deserialize<JsonElement[]>(json)!;
            var instances = arr.Select(InstanceInfo.FromJson).ToList();
            return new InstanceCache(instances, DateTimeOffset.UtcNow);
        }, cancellationToken: cancellationToken);
        _caches[groupId] = cache;
        return cache;
    }

    public InstanceInfo GetInstanceById(string groupId, string instId)
    {
        if (!_caches.TryGetValue(groupId, out var cache))
            throw new InvalidOperationException(LocaleManager.Get("S.ErrNoCacheMsg"));

        return cache.Instances.FirstOrDefault(i => i.Id == instId)
            ?? throw new InvalidOperationException(LocaleManager.Get("S.ErrInstanceNotFoundMsg"));
    }
}
