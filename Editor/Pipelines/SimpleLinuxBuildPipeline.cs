using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleLinuxBuildPipeline : AbstractSimpleBuildPipeline {
        public SimpleLinuxBuildPipeline(string[] scenes) : base(BuildTarget.StandaloneLinux64) {
            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine(PlayerNameNoSpaces, $"{PlayerNameNoSpaces}.x86_64"),
                BuildOptions.StrictMode));
            AddStep(new ArchiveTar(
                PlayerNameNoSpaces,
                $"{PlayerNameNoSpaces}_{Target.ToString()}.tgz"));
        }
    }
}
