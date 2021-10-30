using System.IO;
using Software10101.BuildScripting.Editor;
using UnityEditor;
using UnityEngine;

namespace Software10101.BuildScripting.Example {
    public static class Build {
        private const string BuildsDir = "Builds";
        private const uint SteamAppId = 1000;

        private static readonly string[] Scenes = {
            "Assets/Scenes/Example.unity"
        };

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

        [MenuItem("Build/All", false, 9)]
        private static void BuildAll() {
            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleMacBuildPipeline(Scenes),
                new SimpleLinuxBuildPipeline(Scenes),
                new SimpleWindowsBuildPipeline(Scenes));
        }

        [MenuItem("Build/macOS (Steam)", false, 100)]
        private static void BuildMacSteam() {
            (string username, string password) = GetSteamNameAndPassword();

            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleMacSteamBuildPipeline(Scenes, SteamAppId, 1001, username, password));
        }

        [MenuItem("Build/Windows (Steam)", false, 101)]
        private static void BuildWindowsSteam() {
            (string username, string password) = GetSteamNameAndPassword();

            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleWindowsSteamBuildPipeline(Scenes, SteamAppId, 1002, username, password));
        }

        [MenuItem("Build/Linux (Steam)", false, 102)]
        private static void BuildLinuxSteam() {
            (string username, string password) = GetSteamNameAndPassword();

            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleLinuxSteamBuildPipeline(Scenes, SteamAppId, 1003, username, password));
        }

        [MenuItem("Build/All (Steam)", false, 109)]
        private static void BuildAllSteam() {
            (string username, string password) = GetSteamNameAndPassword();

            CustomBuildPipelineRunner.Execute(
                BuildsDir,
                new SimpleMacSteamBuildPipeline(Scenes, SteamAppId, 1001, username, password),
                new SimpleLinuxSteamBuildPipeline(Scenes, SteamAppId, 1003, username, password),
                new SimpleWindowsSteamBuildPipeline(Scenes, SteamAppId, 1002, username, password));
        }

        [MenuItem("Build/Clean", false, 200)]
        private static void Clean() {
            if (Directory.Exists(BuildsDir)) {
                Directory.Delete(BuildsDir, true);
            }

            Debug.Log("Cleaning complete.");
        }

        private static (string, string) GetSteamNameAndPassword() {
            string[] lines = File.ReadAllLines(@"SteamCreds\user_pw.txt");
            return (lines[0], lines[1]);
        }
    }
}
