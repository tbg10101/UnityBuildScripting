using System.IO;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class BuildPlayerStep : AbstractBuildStep {
        private readonly string[] _scenes;
        private readonly string _outputPath;
        private readonly BuildOptions _options;

        /// <param name="scenes">List of scenes to include in the build.</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="options">Build options.</param>
        public BuildPlayerStep(string[] scenes, string outputPath, BuildOptions options) : base(true) {
            _scenes = scenes;
            _outputPath = outputPath;
            _options = options;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            string outputPath = Path.Combine(outputDir, pipeline.Target.ToString(), _outputPath);
            BuildPipeline.BuildPlayer(_scenes, outputPath, pipeline.Target, _options);
        }
    }
}
