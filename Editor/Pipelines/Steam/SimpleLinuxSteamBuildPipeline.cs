using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleLinuxSteamBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneLinux64;

        private bool _usePlayerLogBackup;

        public SimpleLinuxSteamBuildPipeline(
            string[] scenes,
            uint appId,
            uint depotId,
            string accountName,
            string accountPassword,
            string ssfnPath,
            string configVdfPath,
            string steamCliPath
        ) : base($"{Target.ToString()}_Steam") {
            AddStep(new CustomStep(
                () => {
                    _usePlayerLogBackup = PlayerSettings.usePlayerLog;
                    PlayerSettings.usePlayerLog = true;
                },
                true));

            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine("content", $"{PlayerNameNoSpaces}.x86_64"),
                Target,
                BuildOptions.StrictMode));

            AddStep(new CustomStep(
                () => {
                    PlayerSettings.usePlayerLog = _usePlayerLogBackup;
                },
                true));

            AddStep(new DeployToSteamStep(
                appId,
                depotId,
                "content",
                accountName,
                accountPassword,
                ssfnPath,
                configVdfPath,
                steamCliPath));
        }
    }
}
