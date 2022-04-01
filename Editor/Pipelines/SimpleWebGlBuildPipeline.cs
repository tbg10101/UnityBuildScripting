using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleWebGlBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.WebGL;

        private bool _usePlayerLogBackup;

        public SimpleWebGlBuildPipeline(string[] scenes) : base(Target.ToString()) {
            AddStep(new CustomStep(
                () => {
                    _usePlayerLogBackup = PlayerSettings.usePlayerLog;
                    PlayerSettings.usePlayerLog = true;
                },
                true));

            AddStep(new BuildPlayerStep(
                scenes,
                PlayerNameNoSpaces,
                Target,
                BuildOptions.StrictMode,
                "DISABLESTEAMWORKS"));

            AddStep(new CustomStep(
                () => {
                    PlayerSettings.usePlayerLog = _usePlayerLogBackup;
                },
                true));
        }
    }
}
