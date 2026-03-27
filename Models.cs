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
            displayName = string.IsNullOrEmpty(dn.GetString()) ? null : dn.GetString();

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

public record GroupItem(string Id, string Name);

public static class GroupDefinitions
{
    public static readonly GroupItem[] Groups =
    [
        new("grp_f664b62c-df1a-4ad4-a1df-2b9df679bc04", "スキルブッパ連合"),
        new("grp_746f4574-b608-41d3-baed-03fa906391d5", "スキルブッパ会"),
        new("grp_5900a25d-0bb9-48d4-bab1-f3bd5c9a5e73", "でかプ同好会"),
    ];
}
