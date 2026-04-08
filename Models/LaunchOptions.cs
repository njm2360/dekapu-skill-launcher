namespace DekapuSkillLauncher.Models;

public class LaunchOptions
{
    // Basic
    public bool VrEnabled { get; set; } = false;
    public string ExtraArgs { get; set; } = "";
    public int Fps { get; set; } = 0;            // 0 = no limit

    // OSC
    public bool OscEnabled { get; set; } = false;
    public int OscInPort { get; set; } = 9001;
    public string OscOutIp { get; set; } = "127.0.0.1";
    public int OscOutPort { get; set; } = 9000;

    // Performance
    public string Affinity { get; set; } = "";
    public string ProcessPriority { get; set; } = "";   // empty or -2..2
    public string MainThreadPriority { get; set; } = ""; // empty or -2..2

    // Display (Unity engine options)
    public int ScreenWidth { get; set; } = 0;         // 0 = default
    public int ScreenHeight { get; set; } = 0;        // 0 = default
    public int ScreenFullscreen { get; set; } = -1;   // -1 = default, 0 = windowed, 1 = fullscreen
    public int Monitor { get; set; } = 0;             // 0 = default (1-based index)
    public string HwVideoDecoding { get; set; } = ""; // "" / "disable" / "enable"
    public bool DisableAmdStutterWorkaround { get; set; } = false;

    // Debug
    public bool WatchWorlds { get; set; } = false;
    public bool WatchAvatars { get; set; } = false;
    public bool DebugGui { get; set; } = false;
    public bool SdkLogLevels { get; set; } = false;
    public bool UdonDebugLogging { get; set; } = false;
    public bool SkipRegistryInstall { get; set; } = false;
    public bool EnforceWorldServerChecks { get; set; } = false;
    public string IgnoreTrackers { get; set; } = "";
    public string Midi { get; set; } = "";

    public LaunchOptions Clone() => (LaunchOptions)MemberwiseClone();

    public IEnumerable<string> BuildArguments()
    {
        // Basic
        if (!VrEnabled)
            yield return "--no-vr";
        if (Fps > 0)
            yield return $"--fps={Fps}";

        // OSC
        if (OscEnabled)
            yield return $"--osc={OscInPort}:{OscOutIp}:{OscOutPort}";

        // Performance
        if (!string.IsNullOrEmpty(Affinity))
            yield return $"--affinity={Affinity}";
        if (!string.IsNullOrEmpty(ProcessPriority))
            yield return $"--process-priority={ProcessPriority}";
        if (!string.IsNullOrEmpty(MainThreadPriority))
            yield return $"--main-thread-priority={MainThreadPriority}";

        // Display
        if (ScreenWidth > 0) { yield return "-screen-width"; yield return ScreenWidth.ToString(); }
        if (ScreenHeight > 0) { yield return "-screen-height"; yield return ScreenHeight.ToString(); }
        if (ScreenFullscreen >= 0) { yield return "-screen-fullscreen"; yield return ScreenFullscreen.ToString(); }
        if (Monitor > 0) { yield return "-monitor"; yield return Monitor.ToString(); }
        if (HwVideoDecoding == "disable")
            yield return "--disable-hw-video-decoding";
        else if (HwVideoDecoding == "enable")
            yield return "--enable-hw-video-decoding";
        if (DisableAmdStutterWorkaround)
            yield return "--disable-amd-stutter-workaround";

        // Debug
        if (WatchWorlds) yield return "--watch-worlds";
        if (WatchAvatars) yield return "--watch-avatars";
        if (DebugGui) yield return "--enable-debug-gui";
        if (SdkLogLevels) yield return "--enable-sdk-log-levels";
        if (UdonDebugLogging) yield return "--enable-udon-debug-logging";
        if (SkipRegistryInstall) yield return "--skip-registry-install";
        if (EnforceWorldServerChecks) yield return "--enforce-world-server-checks";
        if (!string.IsNullOrEmpty(IgnoreTrackers))
            yield return $"--ignore-trackers={IgnoreTrackers}";
        if (!string.IsNullOrEmpty(Midi))
            yield return $"--midi={Midi}";

        // Extra
        foreach (var arg in ExtraArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            yield return arg;
    }
}
