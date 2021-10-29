using System;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class Archive7Zip : AbstractBuildStep {
        private const string WindowsApplication = @"C:\Program Files\7-Zip\7z.exe";

        private readonly string _directoryToArchive;
        private readonly string _outputPath;
        private readonly string _type;

        /// <param name="directoryToArchive">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="type">The type of archive to make.</param>
        public Archive7Zip(string directoryToArchive, string outputPath, string type) {
            _directoryToArchive = directoryToArchive;
            _outputPath = outputPath;
            _type = type;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            string args = $"a -r -t{_type} \"{_outputPath}\" \"{_directoryToArchive}\"";
            Debug.Log($"Beginning archive: {WindowsApplication} {args}");

            Process archiveProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = WindowsApplication,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, outputDir, pipeline.Target.ToString())
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
