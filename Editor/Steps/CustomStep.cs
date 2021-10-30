using System;

namespace Software10101.BuildScripting.Editor {
    public class CustomStep : AbstractBuildStep {
        private readonly Action _action;

        public CustomStep(Action action, bool useMainThread = false) : base(useMainThread) {
            _action = action;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            _action?.Invoke();
        }
    }
}
