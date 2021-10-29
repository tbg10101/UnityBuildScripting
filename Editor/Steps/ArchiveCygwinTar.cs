using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class ArchiveCygwinTar : AbstractBuildStep {
        private const string ApplicationPath = @"C:\cygwin\bin\tar.exe";

        private readonly string _directoryToArchive;
        private readonly string _outputPath;
        private readonly bool _compress;

        /// <param name="directoryToArchive">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="compress">Whether or not to compress the archive with gzip.</param>
        public ArchiveCygwinTar(string directoryToArchive, string outputPath, bool compress = true) {
            _directoryToArchive = directoryToArchive;
            _outputPath = outputPath;
            _compress = compress;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
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
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, outputDir, pipeline.Target.ToString())
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
