using System;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class ArchiveCygwinTar : AbstractBuildStep {

        public static string TarPath = @"C:\cygwin\bin\tar.exe";
        public static string GzipPath = @"C:\cygwin\bin\gzip.exe";

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
            string tarFileName = $"{Path.GetFileNameWithoutExtension(_outputPath)}.tar";

            string tarArgs = $"-cf \"{tarFileName}\" \"{_directoryToArchive}\"";
            Debug.Log($"Beginning archive: {TarPath} {tarArgs}");

            Process archiveProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = TarPath,
                    Arguments = tarArgs,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, outputDir)
                }
            };

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

            if (!_compress) {
                File.Move(
                    Path.Combine(outputDir, tarFileName),
                    Path.Combine(outputDir, _outputPath));
                return;
            }

            string compressArgs = $"\"{tarFileName}\"";
            Debug.Log($"Beginning compress: {GzipPath} {compressArgs}");

            Process compressProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = GzipPath,
                    Arguments = compressArgs,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, outputDir)
                }
            };

            compressProcess.OutputDataReceived += (_,  message) => {
                if (string.IsNullOrEmpty(message.Data))
                {
                    return;
                }

                Debug.Log(message.Data);
            };

            compressProcess.ErrorDataReceived += (_,  message) => {
                if (string.IsNullOrEmpty(message.Data))
                {
                    return;
                }

                Debug.LogError(message.Data);
            };

            compressProcess.Start();
            compressProcess.BeginOutputReadLine();
            compressProcess.BeginErrorReadLine();

            compressProcess.WaitForExit();

            if (compressProcess.ExitCode == 0) {
                Debug.Log("Compression exited successfully.");
            } else {
                throw new Exception($"Compression exited with code: {compressProcess.ExitCode}");
            }

            File.Move(
                Path.Combine(outputDir, $"{tarFileName}.gz"),
                Path.Combine(outputDir, _outputPath));
        }
    }
}
