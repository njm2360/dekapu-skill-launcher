using System.Text.Json;

namespace DekapuSkillLauncher;

public record InstanceInfo(
    string Id,
    string Name,
    string? DisplayName,
    int UserCount,
    DateTimeOffset? ClosedAt,
    string WorldId,
    string InstanceId,
    string ShortName)
{
    public static InstanceInfo FromJson(JsonElement d)
    {
        DateTimeOffset? closedAt = null;
        if (d.TryGetProperty("closedAt", out var ca) && ca.ValueKind == JsonValueKind.String)
            closedAt = DateTimeOffset.Parse(ca.GetString()!);

        string? displayName = null;
        if (d.TryGetProperty("displayName", out var dn) && dn.ValueKind == JsonValueKind.String)
            displayName = dn.GetString();

        return new InstanceInfo(
            Id: d.GetProperty("id").GetString()!,
            Name: d.GetProperty("name").GetString()!,
            DisplayName: displayName,
            UserCount: d.GetProperty("userCount").GetInt32(),
            ClosedAt: closedAt,
            WorldId: d.GetProperty("worldId").GetString()!,
            InstanceId: d.GetProperty("instanceId").GetString()!,
            ShortName: d.GetProperty("secureName").GetString()!
        );
    }
}

public record InstanceCache(List<InstanceInfo> Instances, DateTimeOffset UpdatedAt);
