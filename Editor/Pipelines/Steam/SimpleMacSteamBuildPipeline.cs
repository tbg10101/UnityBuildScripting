using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleMacSteamBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneOSX;

        private bool _createXcodeProjectBackup;
        private bool _useMacAppStoreValidationBackup;
        private bool _usePlayerLogBackup;

        public SimpleMacSteamBuildPipeline(
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
                    _createXcodeProjectBackup = UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject;
                    UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = false;

                    _useMacAppStoreValidationBackup = PlayerSettings.useMacAppStoreValidation;
                    PlayerSettings.useMacAppStoreValidation = false;

                    _usePlayerLogBackup = PlayerSettings.usePlayerLog;
                    PlayerSettings.usePlayerLog = true;

                    PlayerSettings.macOS.buildNumber = (ulong.Parse(PlayerSettings.macOS.buildNumber) + 1).ToString();
                },
                true));

            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine("content", $"{PlayerName}.app"),
                Target,
                BuildOptions.StrictMode));

            AddStep(new CustomStep(
                () => {
                    UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = _createXcodeProjectBackup;
                    PlayerSettings.useMacAppStoreValidation = _useMacAppStoreValidationBackup;
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
