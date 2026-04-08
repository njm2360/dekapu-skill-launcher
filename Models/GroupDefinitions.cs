namespace DekapuSkillLauncher.Models;

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
