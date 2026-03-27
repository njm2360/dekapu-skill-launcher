using System.Net.Http;
using System.Text.Json;

namespace DekapuSkillLauncher;

public class InstanceService(AppSettings settings)
{
    private static readonly HttpClient _client = new() { Timeout = TimeSpan.FromSeconds(30) };
    private readonly Dictionary<string, InstanceCache> _caches = new();

    public async Task<InstanceCache> GetInstancesAsync(string groupId, bool refresh = false)
    {
        if (!refresh && _caches.TryGetValue(groupId, out var cached))
            return cached;

        var json = await _client.GetStringAsync($"{settings.ApiBaseUrl}/instances/{groupId}");
        var arr = JsonSerializer.Deserialize<JsonElement[]>(json)!;
        var instances = arr.Select(InstanceInfo.FromJson).ToList();
        var cache = new InstanceCache(instances, DateTimeOffset.UtcNow);
        _caches[groupId] = cache;
        return cache;
    }

    public InstanceInfo GetInstanceById(string groupId, string instId)
    {
        if (!_caches.TryGetValue(groupId, out var cache))
            throw new InvalidOperationException("キャッシュが存在しません");

        return cache.Instances.FirstOrDefault(i => i.Id == instId)
            ?? throw new InvalidOperationException("インスタンスが見つかりません");
    }
}
