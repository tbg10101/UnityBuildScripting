using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleWindowsSteamBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneWindows;

        public SimpleWindowsSteamBuildPipeline(
            string[] scenes,
            uint appId,
            uint depotId,
            string accountName,
            string accountPassword
        ) : base($"{Target.ToString()}_Steam") {
            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine("content", $"{PlayerName}.exe"),
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
