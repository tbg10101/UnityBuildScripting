using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class SimpleWindowsBuildPipeline : AbstractSimpleBuildPipeline {
        public SimpleWindowsBuildPipeline(string[] scenes) : base(BuildTarget.StandaloneWindows) {
            AddStep(new BuildPlayerStep(
                scenes,
                Path.Combine(PlayerName, $"{PlayerName}.exe"),
                BuildOptions.StrictMode));
            AddStep(new Archive7Zip(
                PlayerName,
                $"{PlayerNameNoSpaces}_{Target.ToString()}.zip",
                "zip"));
        }
    }
}
