using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleMacSteamBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneOSX;

        public SimpleMacSteamBuildPipeline(
            string[] scenes,
            uint appId,
            uint depotId,
            string accountName,
            string accountPassword
        ) : base($"{Target.ToString()}_Steam") {
            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine("content", $"{PlayerName}.app"),
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
