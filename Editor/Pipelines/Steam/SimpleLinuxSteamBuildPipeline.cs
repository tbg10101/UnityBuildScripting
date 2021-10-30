using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleLinuxSteamBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneLinux64;

        public SimpleLinuxSteamBuildPipeline(
            string[] scenes,
            uint appId,
            uint depotId,
            string accountName,
            string accountPassword
        ) : base($"{Target.ToString()}_Steam") {
            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine("content", $"{PlayerNameNoSpaces}.x86_64"),
                Target,
                BuildOptions.StrictMode));
            AddStep(new DeployToSteamStep(
                appId,
                depotId,
                "content",
                accountName,
                accountPassword));
        }
    }
}
