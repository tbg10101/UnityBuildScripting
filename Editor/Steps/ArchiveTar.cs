using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class ArchiveTar : AbstractBuildStep {
        private const string ApplicationPath = "tar";

        private readonly string _directoryToArchive;
        private readonly string _outputPath;
        private readonly bool _compress;

        /// <param name="directoryToArchive">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="compress">Whether or not to compress the archive with gzip.</param>
        public ArchiveTar(string directoryToArchive, string outputPath, bool compress = true) {
            _directoryToArchive = directoryToArchive;
            _outputPath = outputPath;
            _compress = compress;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                if (pipeline.Target == BuildTarget.StandaloneLinux64 || pipeline.Target == BuildTarget.StandaloneOSX) {
                    Debug.LogWarning("tar on Windows does not preserve POSIX permissions!");
                }
            }

            string args = $"-c{(_compress ? "z" : "")}f \"{_outputPath}\" \"{_directoryToArchive}\"";
            Debug.Log($"Beginning archive: {ApplicationPath} {args}");

            Process archiveProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = ApplicationPath,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, outputDir)
                }
            };

            archiveProcess.OutputDataReceived += (_,  message) => Debug.Log(message.Data);
            archiveProcess.ErrorDataReceived += (_,  message) => Debug.LogError(message.Data);

            archiveProcess.Start();
            archiveProcess.WaitForExit();

            if (archiveProcess.ExitCode == 0) {
                Debug.Log("Archive exited successfully.");
            } else {
                throw new Exception($"Archive exited with code: {archiveProcess.ExitCode}");
            }
        }
    }
}
