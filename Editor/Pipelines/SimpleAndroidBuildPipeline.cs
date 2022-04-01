using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleAndroidBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.Android;

        private bool _usePlayerLogBackup;

        public SimpleAndroidBuildPipeline(
            string[] scenes,
            string keystorePassword,
            string keyaliasPassword
        ) : base(Target.ToString()) {
            AddStep(new CustomStep(
                () => {
                    _usePlayerLogBackup = PlayerSettings.usePlayerLog;
                    PlayerSettings.usePlayerLog = true;

                    PlayerSettings.Android.keystorePass = keystorePassword;
                    PlayerSettings.Android.keyaliasPass = keyaliasPassword;

                    PlayerSettings.Android.bundleVersionCode++;
                },
                true));

            AddStep(new BuildPlayerStep(
                scenes,
                $"{PlayerNameNoSpaces}.apk",
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
