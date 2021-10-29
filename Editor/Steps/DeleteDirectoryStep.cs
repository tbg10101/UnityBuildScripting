using System.IO;
using UnityEngine;

namespace Software10101.BuildScripting.Editor {
    public class DeleteDirectoryStep : AbstractBuildStep {
        private readonly string _path;

        /// <param name="path">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        public DeleteDirectoryStep(string path) {
            _path = path;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            try {
                string path = Path.Combine(outputDir, pipeline.Target.ToString(), _path);
                Directory.Delete(path, true);
                Debug.Log($"Deleted directory: {path}");
            } catch (DirectoryNotFoundException) {
                // dont worry about it
            }
        }
    }
}
