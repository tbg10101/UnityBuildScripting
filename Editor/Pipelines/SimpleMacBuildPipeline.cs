using UnityEditor;
using UnityEngine;

namespace Software10101.BuildScripting.Editor {
    public class SimpleMacBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneOSX;

        private bool _createXcodeProjectBackup;
        private bool _useMacAppStoreValidationBackup;
        private bool _usePlayerLogBackup;

        public SimpleMacBuildPipeline(string[] scenes) : base(Target.ToString()) {
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

            string playerOutputPath = $"{PlayerName}.app";

            AddStep(new BuildPlayerStep(
                scenes,
                playerOutputPath,
                Target,
                BuildOptions.StrictMode,
                "DISABLESTEAMWORKS"));

            AddStep(new CustomStep(
                () => {
                    UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = _createXcodeProjectBackup;
                    PlayerSettings.useMacAppStoreValidation = _useMacAppStoreValidationBackup;
                    PlayerSettings.usePlayerLog = _usePlayerLogBackup;
                },
                true));

            string tgzOutputPath = $"{PlayerNameNoSpaces}_{Target.ToString()}.tgz";

            if (Application.platform == RuntimePlatform.WindowsEditor) {
                AddStep(new ArchiveCygwinTar(playerOutputPath, tgzOutputPath));
            } else {
                AddStep(new ArchiveTar(playerOutputPath, tgzOutputPath));
            }
        }
    }
}
