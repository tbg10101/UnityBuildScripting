namespace Software10101.BuildScripting.Editor {
    public abstract class AbstractBuildStep {
        internal readonly bool UseMainThread;

        public AbstractBuildStep(bool useMainThread = false) {
            UseMainThread = useMainThread;
        }

        public abstract void Execute(string outputDir, AbstractBuildPipeline pipeline);
    }
}
