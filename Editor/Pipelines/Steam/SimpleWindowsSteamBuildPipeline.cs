using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleWindowsSteamBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneWindows;

        private bool _createSolutionBackup;
        private bool _usePlayerLogBackup;

        public SimpleWindowsSteamBuildPipeline(
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
                    _createSolutionBackup = UnityEditor.WindowsStandalone.UserBuildSettings.createSolution;
                    UnityEditor.WindowsStandalone.UserBuildSettings.createSolution = false;

                    _usePlayerLogBackup = PlayerSettings.usePlayerLog;
                    PlayerSettings.usePlayerLog = true;
                },
                true));

            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine("content", $"{PlayerName}.exe"),
                Target,
                BuildOptions.StrictMode));

            AddStep(new CustomStep(
                () => {
                    UnityEditor.WindowsStandalone.UserBuildSettings.createSolution = _createSolutionBackup;
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
