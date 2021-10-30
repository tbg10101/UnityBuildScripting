using System.IO;
using UnityEditor;
using UnityEngine;

namespace Software10101.BuildScripting.Editor {
    public class SimpleLinuxBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneLinux64;

        public SimpleLinuxBuildPipeline(string[] scenes) : base(Target.ToString()) {
            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine(PlayerNameNoSpaces, $"{PlayerNameNoSpaces}.x86_64"),
                Target,
                BuildOptions.StrictMode));
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                AddStep(new ArchiveCygwinTar(
                    PlayerNameNoSpaces,
                    $"{PlayerNameNoSpaces}_{Target.ToString()}.tgz"));
            } else {
                AddStep(new ArchiveTar(
                    PlayerNameNoSpaces,
                    $"{PlayerNameNoSpaces}_{Target.ToString()}.tgz"));
            }
        }
    }
}
