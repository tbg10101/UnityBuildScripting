using System.IO;
using Software10101.BuildScripting.Editor;
using UnityEditor;
using UnityEngine;

namespace Software10101.BuildScripting.Example {
    public static class Build {
        private const string BuildsDir = "Builds";

        private static readonly string[] Scenes = {
            "Assets/Scenes/Example.unity"
        };

        #region Steam Parameters
        private const string SteamCliPath = @"C:\Program Files\Steamworks SDK\tools\ContentBuilder\builder\steamcmd.exe";

        private const uint SteamAppId = 1000;
        private const uint MacDepotId = 1001;
        private const uint WindowsDepotId = 1001;
        private const uint LinuxDepotId = 1001;

        // normally you would get these from a secrets store
        private const string SteamUsername = "username";
        private const string SteamPassword = "password";
        private const string SsfnPath = @"SteamCreds\ssfn12345678901234567890";
        private const string ConfigVdfPath = @"SteamCreds\config\config.vdf";
        #endregion

        [MenuItem("Build/macOS", false, 0)]
        private static void BuildMac() {
            CustomBuildPipelineRunner.Execute(BuildsDir, new SimpleMacBuildPipeline(Scenes));
        }

        [MenuItem("Build/Windows", false, 1)]
        private static void BuildWindows() {
            CustomBuildPipelineRunner.Execute(BuildsDir, new SimpleWindowsBuildPipeline(Scenes));
        }

        [MenuItem("Build/Linux", false, 2)]
        private static void BuildLinux() {
            CustomBuildPipelineRunner.Execute(BuildsDir, new SimpleLinuxBuildPipeline(Scenes));
        }

        [MenuItem("Build/All (Standalone)", false, 9)]
        private static void BuildAll() {
            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleMacBuildPipeline(Scenes),
                new SimpleLinuxBuildPipeline(Scenes),
                new SimpleWindowsBuildPipeline(Scenes));
        }

        [MenuItem("Build/macOS (Steam)", false, 100)]
        private static void BuildMacSteam() {
            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleMacSteamBuildPipeline(
                    Scenes,
                    SteamAppId,
                    MacDepotId,
                    SteamUsername,
                    SteamPassword,
                    SsfnPath,
                    ConfigVdfPath,
                    SteamCliPath));
        }

        [MenuItem("Build/Windows (Steam)", false, 101)]
        private static void BuildWindowsSteam() {
            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleWindowsSteamBuildPipeline(
                    Scenes,
                    SteamAppId,
                    WindowsDepotId,
                    SteamUsername,
                    SteamPassword,
                    SsfnPath,
                    ConfigVdfPath,
                    SteamCliPath));
        }

        [MenuItem("Build/Linux (Steam)", false, 102)]
        private static void BuildLinuxSteam() {
            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleLinuxSteamBuildPipeline(
                    Scenes,
                    SteamAppId,
                    LinuxDepotId,
                    SteamUsername,
                    SteamPassword,
                    SsfnPath,
                    ConfigVdfPath,
                    SteamCliPath));
        }

        [MenuItem("Build/All (Steam)", false, 109)]
        private static void BuildAllSteam() {
            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleMacSteamBuildPipeline(
                    Scenes, SteamAppId,
                    MacDepotId,
                    SteamUsername,
                    SteamPassword,
                    SsfnPath,
                    ConfigVdfPath,
                    SteamCliPath),
                new SimpleLinuxSteamBuildPipeline(
                    Scenes,
                    SteamAppId,
                    LinuxDepotId,
                    SteamUsername,
                    SteamPassword,
                    SsfnPath,
                    ConfigVdfPath,
                    SteamCliPath),
                new SimpleWindowsSteamBuildPipeline(
                    Scenes,
                    SteamAppId,
                    WindowsDepotId,
                    SteamUsername,
                    SteamPassword,
                    SsfnPath,
                    ConfigVdfPath,
                    SteamCliPath));
        }

        [MenuItem("Build/Android", false, 200)]
        private static void BuildAndroid() {
            // normally you would get this from a secrets store
            const string keystorePassword = "666666";

            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleAndroidBuildPipeline(Scenes, keystorePassword, keystorePassword));
        }

        [MenuItem("Build/WebGL", false, 300)]
        private static void BuildWebGl() {
            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleWebGlBuildPipeline(Scenes));
        }

        [MenuItem("Build/Clean", false, 100000)]
        private static void Clean() {
            if (Directory.Exists(BuildsDir)) {
                Directory.Delete(BuildsDir, true);
            }

            Debug.Log("Cleaning complete.");
        }
    }
}
