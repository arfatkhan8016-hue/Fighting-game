using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AetherBlade.Editor
{
    public static class BuildScript
    {
        public static void BuildAndroid()
        {
            PlayerSettings.companyName = "AetherStudio";
            PlayerSettings.productName = "Aether Blade";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.aetherstudio.aetherblade");
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.Android, ManagedStrippingLevel.High);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.Android.bundleVersionCode = int.TryParse(
                System.Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER"), out var n) ? n : 1;
            EditorUserBuildSettings.buildAppBundle = false;

            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length == 0)
            {
                SceneBuilder.Build();
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            }

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = "build/Android/AetherBlade.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) EditorApplication.Exit(1);
        }
    }
}
