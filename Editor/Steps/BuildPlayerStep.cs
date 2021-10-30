using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class BuildPlayerStep : AbstractBuildStep {
        private readonly string[] _scenes;
        private readonly string _outputPath;
        private readonly BuildOptions _options;
        private readonly BuildTarget _target;

        /// <param name="scenes">List of scenes to include in the build.</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="target">The platform to build for.</param>
        /// <param name="options">Build options.</param>
        public BuildPlayerStep(string[] scenes, string outputPath, BuildTarget target, BuildOptions options) : base(true) {
            _scenes = scenes;
            _outputPath = outputPath;
            _options = options;
            _target = target;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            string outputPath = Path.Combine(outputDir, _outputPath);
            BuildPipeline.BuildPlayer(_scenes, outputPath, _target, _options);
        }
    }
}
