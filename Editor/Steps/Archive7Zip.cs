using System;
using System.Diagnostics;
using System.IO;
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
            Debug.LogWarning("7-Zip does not preserve permissions!");

            string args = $"a -r -t{_type} \"{_outputPath}\" \"{_directoryToArchive}\"";
            Debug.Log($"Beginning archive: {ApplicationPath} {args}");

            using (Process archiveProcess = new Process {
                       StartInfo = new ProcessStartInfo {
                           FileName = ApplicationPath,
                           Arguments = args,
                           CreateNoWindow = true,
                           UseShellExecute = false,
                           RedirectStandardOutput = true,
                           RedirectStandardError = true,
                           WorkingDirectory = Path.Combine(Environment.CurrentDirectory, outputDir)
                       }
                    }
            ) {
                archiveProcess.OutputDataReceived += (_,  message) => {
                    if (string.IsNullOrEmpty(message.Data))
                    {
                        return;
                    }

                    Debug.Log(message.Data);
                };

                archiveProcess.ErrorDataReceived += (_,  message) => {
                    if (string.IsNullOrEmpty(message.Data))
                    {
                        return;
                    }

                    Debug.LogError(message.Data);
                };

                archiveProcess.Start();
                archiveProcess.BeginOutputReadLine();
                archiveProcess.BeginErrorReadLine();

                archiveProcess.WaitForExit();

                if (archiveProcess.ExitCode == 0) {
                    Debug.Log("Archive exited successfully.");
                } else {
                    throw new Exception($"Archive exited with code: {archiveProcess.ExitCode}");
                }
            }
        }
    }
}
