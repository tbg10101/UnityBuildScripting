using UnityEditor;
using UnityEngine;

namespace Software10101.BuildScripting.Editor {
    public class SimpleMacBuildPipeline : AbstractSimpleBuildPipeline {
        public SimpleMacBuildPipeline(string[] scenes) : base(BuildTarget.StandaloneOSX) {
            AddStep(new BuildPlayerStep(
                scenes,
                $"{PlayerNameNoSpaces}.app",
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
