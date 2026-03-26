using System.Net.Http;
using System.Text.Json;

namespace DekapuSkillLauncher;

public class InstanceService(AppSettings settings)
{
    private static readonly HttpClient _client = new() { Timeout = TimeSpan.FromSeconds(30) };
    private InstanceCache? _cache;

    public async Task<InstanceCache> GetInstancesAsync(bool refresh = false)
    {
        if (!refresh && _cache is not null)
            return _cache;

        var json = await _client.GetStringAsync($"{settings.ApiBaseUrl}/instances");
        var arr = JsonSerializer.Deserialize<JsonElement[]>(json)!;
        var instances = arr.Select(InstanceInfo.FromJson).ToList();
        _cache = new InstanceCache(instances, DateTimeOffset.UtcNow);
        return _cache;
    }

    public InstanceInfo GetInstanceById(string instId)
    {
        if (_cache is null)
            throw new InvalidOperationException("キャッシュが存在しません");

        return _cache.Instances.FirstOrDefault(i => i.Id == instId)
            ?? throw new InvalidOperationException("インスタンスが見つかりません");
    }
}
