using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class Archive7Zip : AbstractBuildStep {
        public static string ApplicationPath = @"C:\Program Files\7-Zip\7z.exe";

        private readonly string _directoryToArchive;
        private readonly string _outputPath;
        private readonly string _type;

        /// <param name="directoryToArchive">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="type">The type of archive to make.</param>
        public Archive7Zip(string directoryToArchive, string outputPath, string type) {
            if (Application.platform != RuntimePlatform.WindowsEditor) {
                throw new Exception("7-Zip is only supported on Windows.");
            }

            _directoryToArchive = directoryToArchive;
            _outputPath = outputPath;
            _type = type;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            if (pipeline.Target == BuildTarget.StandaloneLinux64 || pipeline.Target == BuildTarget.StandaloneOSX) {
                Debug.LogWarning("7-Zip does not preserve POSIX permissions!");
            }

            string args = $"a -r -t{_type} \"{_outputPath}\" \"{_directoryToArchive}\"";
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
