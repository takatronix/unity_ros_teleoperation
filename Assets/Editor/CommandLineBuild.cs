using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

public class CommandLineBuild
{
    private static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
    }

    [MenuItem("Build/Android (Quest)")]
    public static void BuildAndroid()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = "Builds/Android/teleoperation.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        Build(options);
    }

    [MenuItem("Build/Linux")]
    public static void BuildLinux()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = "Builds/Linux/teleoperation",
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.None
        };

        Build(options);
    }

    [MenuItem("Build/Windows")]
    public static void BuildWindows()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = "Builds/Windows/teleoperation.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        Build(options);
    }

    [MenuItem("Build/macOS")]
    public static void BuildMacOS()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = "Builds/macOS/teleoperation.app",
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        };

        Build(options);
    }

    private static void Build(BuildPlayerOptions options)
    {
        Debug.Log($"Building for {options.target}...");
        Debug.Log($"Scenes: {string.Join(", ", options.scenes)}");
        Debug.Log($"Output: {options.locationPathName}");

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize} bytes, {summary.totalTime}");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed!");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }
}
