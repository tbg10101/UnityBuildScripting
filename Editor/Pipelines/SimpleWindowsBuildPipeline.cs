using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleWindowsBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneWindows;

        private bool _createSolutionBackup;
        private bool _usePlayerLogBackup;

        public SimpleWindowsBuildPipeline(string[] scenes) : base(Target.ToString()) {
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
                Path.Combine(PlayerName, $"{PlayerName}.exe"),
                Target,
                BuildOptions.StrictMode,
                "DISABLESTEAMWORKS"));

            AddStep(new CustomStep(
                () => {
                    UnityEditor.WindowsStandalone.UserBuildSettings.createSolution = _createSolutionBackup;
                    PlayerSettings.usePlayerLog = _usePlayerLogBackup;
                },
                true));

            AddStep(new Archive7Zip(
                PlayerName,
                $"{PlayerNameNoSpaces}_{Target.ToString()}.zip",
                "zip"));
        }
    }
}
