using Microsoft.Win32;

namespace DekapuSkillLauncher.Services;

public static class StartupManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
    private const string ValueName = "DekapuSkillLauncher";

    public static bool IsEnabled()
    {
        try
        {
            using var run = Registry.CurrentUser.OpenSubKey(RunKeyPath);
            if (run?.GetValue(ValueName) is null)
                return false;

            using var approved = Registry.CurrentUser.OpenSubKey(ApprovedKeyPath);
            if (approved?.GetValue(ValueName) is byte[] state && state.Length > 0 && (state[0] & 0x01) != 0)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void SetEnabled(bool enabled)
    {
        using var run = Registry.CurrentUser.CreateSubKey(RunKeyPath);
        if (enabled)
        {
            var exePath = Environment.ProcessPath
                ?? throw new InvalidOperationException("Executable path not available");
            run.SetValue(ValueName, $"\"{exePath}\"");
        }
        else
        {
            run.DeleteValue(ValueName, throwOnMissingValue: false);
        }

        using var approved = Registry.CurrentUser.OpenSubKey(ApprovedKeyPath, writable: true);
        approved?.DeleteValue(ValueName, throwOnMissingValue: false);
    }
}
