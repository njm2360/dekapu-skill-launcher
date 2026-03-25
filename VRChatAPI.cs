using System.Text.Json;

namespace SimpleLauncherWinForms;

public class VRChatAPI
{
    private static readonly HttpClient _client = new();
    private const string BaseUrl = "http://10.83.27.100:8080/api";

    public async Task<List<GroupInstance>> GetGroupInstancesAsync(string groupId)
    {
        var json = await _client.GetStringAsync($"{BaseUrl}/groups/{groupId}/instances");
        var arr = JsonSerializer.Deserialize<JsonElement[]>(json)!;
        return arr.Select(GroupInstance.FromJson).ToList();
    }

    public async Task<InstanceInfo> GetInstanceInfoAsync(string worldId, string instanceId)
    {
        var json = await _client.GetStringAsync($"{BaseUrl}/instances/{worldId}:{instanceId}");
        var elem = JsonSerializer.Deserialize<JsonElement>(json);
        return InstanceInfo.FromJson(elem);
    }
}
