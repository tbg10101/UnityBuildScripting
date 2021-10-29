using System;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class ZipArchive : AbstractBuildStep {
        const string Zip = @"C:\Program Files\7-Zip\7z.exe";

        private readonly string _directoryToArchive;
        private readonly string _outputPath;

        /// <param name="directoryToArchive">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        public ZipArchive(string directoryToArchive, string outputPath) {
            _directoryToArchive = directoryToArchive;
            _outputPath = outputPath;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            string directoryToArchive = Path.Combine(outputDir, pipeline.Target.ToString(), _directoryToArchive);
            string outputPath = Path.Combine(outputDir, pipeline.Target.ToString(), _outputPath);

            string args = $"a -r -tzip \"{outputPath}\" \"{directoryToArchive}\"";
            Debug.Log($"Beginning archive: {Zip} {args}");

            Process archiveProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = Zip,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Environment.CurrentDirectory
                }
            };

            archiveProcess.OutputDataReceived += (_,  message) => Debug.Log(message.Data);
            archiveProcess.ErrorDataReceived += (_,  message) => Debug.LogError(message.Data);

            archiveProcess.Start();
            archiveProcess.WaitForExit();

            Debug.Log($"Archive exited with code: {archiveProcess.ExitCode}");
        }
    }
}
