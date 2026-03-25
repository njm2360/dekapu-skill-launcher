namespace SimpleLauncherWinForms;

public class InstanceService
{
    private readonly VRChatAPI _api = new();
    private readonly Dictionary<string, InstanceCache> _cache = [];

    public async Task<InstanceCache> GetGroupInstancesAsync(string groupId, bool refresh = false)
    {
        if (!refresh && _cache.TryGetValue(groupId, out var cached))
            return cached;

        var instances = new List<InstanceInfo>();
        foreach (var gi in await _api.GetGroupInstancesAsync(groupId))
        {
            try
            {
                var inst = await _api.GetInstanceInfoAsync(gi.World.Id, gi.InstanceId);
                instances.Add(inst);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"インスタンス取得失敗: {ex.Message}");
            }
        }

        var cache = new InstanceCache(instances, DateTimeOffset.UtcNow);
        _cache[groupId] = cache;
        return cache;
    }

    public InstanceInfo GetInstanceById(string groupId, string instId)
    {
        if (!_cache.TryGetValue(groupId, out var cache))
            throw new InvalidOperationException("キャッシュが存在しません");

        return cache.Instances.FirstOrDefault(i => i.Id == instId)
            ?? throw new InvalidOperationException("インスタンスが見つかりません");
    }
}
