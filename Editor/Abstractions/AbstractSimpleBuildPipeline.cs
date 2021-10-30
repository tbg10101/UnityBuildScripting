using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public abstract class AbstractSimpleBuildPipeline : AbstractBuildPipeline {
        protected readonly string PlayerName = PlayerSettings.productName;
        protected readonly string PlayerNameNoSpaces = PlayerSettings.productName.Replace(" ", "");

        protected AbstractSimpleBuildPipeline(string name) : base(name) {
            AddStep(new DeleteDirectoryStep(""));
        }
    }
}
