using UnityEditor;
using UnityEngine;

namespace Software10101.BuildScripting.Editor {
    public class SimpleMacBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneOSX;

        public SimpleMacBuildPipeline(string[] scenes) : base(Target.ToString()) {
            AddStep(new BuildPlayerStep(
                scenes,
                $"{PlayerNameNoSpaces}.app",
                Target,
                BuildOptions.StrictMode));
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                AddStep(new ArchiveCygwinTar(
                    $"{PlayerNameNoSpaces}.app",
                    $"{PlayerNameNoSpaces}_{Target.ToString()}.tgz"));
            } else {
                AddStep(new ArchiveTar(
                    $"{PlayerNameNoSpaces}.app",
                    $"{PlayerNameNoSpaces}_{Target.ToString()}.tgz"));
            }
        }
    }
}
