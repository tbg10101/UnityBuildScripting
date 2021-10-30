using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleWindowsBuildPipeline : AbstractSimpleBuildPipeline {
        private const BuildTarget Target = BuildTarget.StandaloneWindows;

        public SimpleWindowsBuildPipeline(string[] scenes) : base(Target.ToString()) {
            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine(PlayerName, $"{PlayerName}.exe"),
                Target,
                BuildOptions.StrictMode));
            AddStep(new Archive7Zip(
                PlayerName,
                $"{PlayerNameNoSpaces}_{Target.ToString()}.zip",
                "zip"));
        }
    }
}
