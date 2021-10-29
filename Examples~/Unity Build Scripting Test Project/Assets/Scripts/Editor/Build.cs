using Software10101.BuildScripting.Editor;
using UnityEditor;

namespace Software10101.BuildScripting.Example {
    public static class Build {
        private static readonly string[] Scenes = {
            "Assets/Scenes/Example.unity"
        };

        [MenuItem("Build/macOS", false, 0)]
        private static void BuildMac() {
            CustomBuildPipelineRunner.Execute("Builds", new SimpleMacBuildPipeline(Scenes));
        }

        [MenuItem("Build/Windows", false, 1)]
        private static void BuildWindows() {
            CustomBuildPipelineRunner.Execute("Builds", new SimpleWindowsBuildPipeline(Scenes));
        }

        [MenuItem("Build/Linux", false, 2)]
        private static void BuildLinux() {
            CustomBuildPipelineRunner.Execute("Builds", new SimpleLinuxBuildPipeline(Scenes));
        }

        [MenuItem("Build/All", false, 100)]
        private static void BuildAll() {
            CustomBuildPipelineRunner.Execute(
                "Builds",
                new SimpleMacBuildPipeline(Scenes),
                new SimpleLinuxBuildPipeline(Scenes),
                new SimpleWindowsBuildPipeline(Scenes));
        }
    }
}
