using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleMacBuildPipeline : AbstractSimpleBuildPipeline {
        public SimpleMacBuildPipeline(string[] scenes) : base(BuildTarget.StandaloneOSX) {
            AddStep(new BuildPlayerStep(
                scenes,
                $"{PlayerNameNoSpaces}.app",
                BuildOptions.StrictMode));
            AddStep(new ZipArchive(
                $"{PlayerNameNoSpaces}.app",
                $"{PlayerNameNoSpaces}_{Target.ToString()}.zip"));
        }
    }
}
