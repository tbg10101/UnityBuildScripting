using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class DeployToSteamStep : AbstractBuildStep {
        // Ensure that we only run one of these at a time or Steam can fail.
        private static readonly object LockObject = new object();

        private readonly uint _appId;
        private readonly uint _depotId;
        [NotNull]
        private readonly string _contentRoot;
        [NotNull]
        private readonly string _accountName;
        [NotNull]
        private readonly string _accountPassword;
        [NotNull]
        private readonly string _ssfnPath;
        [NotNull]
        private readonly string _configVdfPath;
        [NotNull]
        private readonly string _steamCliPath;

        /// <param name="appId">Steam app ID.</param>
        /// <param name="depotId">Steam depot ID where the build will be updated.</param>
        /// <param name="contentRoot">The root directory to upload to Steam.</param>
        /// <param name="accountName">The username of the Steam account to use when uploading the build.</param>
        /// <param name="accountPassword">The password for the given account.</param>
        /// <param name="ssfnPath">
        ///     Used to get around SteamGuard. To get this file, log into Steam and enter your Steam Guard code like normal.
        ///     A file with a name starting with "ssfn" be generated next to the Steam executable.
        ///
        ///     https://partner.steamgames.com/doc/sdk/uploading
        /// </param>
        /// <param name="configVdfPath">
        ///     Used to get around SteamGuard. To get this file, log into Steam and enter your Steam Guard code like normal.
        ///     A file named "config.vdf" will be generated in a directory called "config" next to the Steam executable.
        ///
        ///     https://partner.steamgames.com/doc/sdk/uploading
        /// </param>
        /// <param name="steamCliPath">Path to the Steam CLI to use when uploading the build.</param>
        public DeployToSteamStep(
            uint appId,
            uint depotId,
            string contentRoot,
            string accountName,
            string accountPassword,
            string ssfnPath,
            string configVdfPath,
            string steamCliPath
        ) {
            _appId = appId;
            _depotId = depotId;
            _contentRoot = contentRoot
                           ?? throw new ArgumentException("Parameter cannot be null.", nameof(contentRoot));
            _accountName = accountName
                           ?? throw new ArgumentException("Parameter cannot be null.", nameof(accountName));
            _accountPassword = accountPassword
                               ?? throw new ArgumentException("Parameter cannot be null.", nameof(accountPassword));
            _ssfnPath = ssfnPath
                        ?? throw new ArgumentException("Parameter cannot be null.", nameof(ssfnPath));
            _configVdfPath = configVdfPath
                             ?? throw new ArgumentException("Parameter cannot be null.", nameof(configVdfPath));
            _steamCliPath = steamCliPath
                            ?? throw new ArgumentException("Parameter cannot be null.", nameof(steamCliPath));
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            lock (LockObject) {
                // create the VDF file for Steam
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("\"AppBuild\"");
                sb.AppendLine("{");
                {
                    sb.AppendLine($"	\"AppID\" \"{_appId}\"");
                    sb.AppendLine($"	\"Desc\" \"UnityBuildScripting-{pipeline.Name}\"");

                    sb.AppendLine($"	\"ContentRoot\" \"{_contentRoot}\"");
                    sb.AppendLine("	\"BuildOutput\" \"steam_logs\"");

                    sb.AppendLine("	\"Depots\"");
                    sb.AppendLine("	{");
                    {
                        sb.AppendLine($"		\"{_depotId}\"");
                        sb.AppendLine("		{");
                        {
                            sb.AppendLine("			\"FileMapping\"");
                            sb.AppendLine("			{");
                            {
                                sb.AppendLine("				\"LocalPath\" \"*\"");
                                sb.AppendLine("				\"DepotPath\" \".\"");
                                sb.AppendLine("				\"recursive\" \"1\"");
                            }
                            sb.AppendLine("			}");
                            sb.AppendLine("			\"FileExclusion\" \"*.pdb\"");
                        }
                        sb.AppendLine("		}");
                    }
                    sb.AppendLine("	}");
                }
                sb.AppendLine("}");

                const string vdfFileName = "app_build.vdf";
                string vdfPath = Path.Combine(outputDir, vdfFileName);
                string vdfFullPath = Path.GetFullPath(vdfPath);

                File.WriteAllText(vdfPath, sb.ToString());

                // write SteamGuard files
                string ssfnDestination = Path.GetFullPath(Path.Combine(_steamCliPath, "..", Path.GetFileName(_ssfnPath)));
                string ssfnBackup = $"{ssfnDestination}.bak";

                if (File.Exists(ssfnDestination)) {
                    if (File.Exists(ssfnBackup)) {
                        File.Delete(ssfnBackup);
                    }

                    File.Move(ssfnDestination, ssfnBackup);
                }

                File.Copy(_ssfnPath, ssfnDestination);

                string configParent = Path.GetFullPath(Path.Combine(_steamCliPath, "..", "config"));

                if (!Directory.Exists(configParent)) {
                    Directory.CreateDirectory(configParent);
                }

                string configDestination = Path.Combine(configParent, Path.GetFileName(_configVdfPath));
                string configBackup = $"{configDestination}.bak";

                if (File.Exists(configDestination)) {
                    if (File.Exists(configBackup)) {
                        File.Delete(configBackup);
                    }

                    File.Move(configDestination, configBackup);
                }

                File.Copy(_configVdfPath, configDestination);

                // execute deployment
                string args =
                    $"+@ShutdownOnFailedCommand 1 +@NoPromptForPassword 1 +login \"{_accountName}\" \"{_accountPassword}\" +run_app_build \"{vdfFullPath}\" +quit";
                Debug.Log($"Beginning deployment: {_steamCliPath} {args}");

                using (Process deployProcess = new Process {
                           StartInfo = new ProcessStartInfo {
                               FileName = _steamCliPath,
                               Arguments = args,
                               CreateNoWindow = true,
                               UseShellExecute = false,
                               RedirectStandardOutput = true,
                               RedirectStandardError = true
                           }
                       }
                      ) {
                    deployProcess.OutputDataReceived += (_,  message) => {
                        if (string.IsNullOrEmpty(message.Data)) {
                            return;
                        }

                        Debug.Log(message.Data);
                    };

                    deployProcess.ErrorDataReceived += (_, message) => {
                        if (string.IsNullOrEmpty(message.Data)) {
                            return;
                        }

                        Debug.LogError(message.Data);
                    };

                    deployProcess.Start();
                    deployProcess.BeginOutputReadLine();
                    deployProcess.BeginErrorReadLine();

                    deployProcess.WaitForExit();

                    if (deployProcess.ExitCode == 0) {
                        Debug.Log("Deployment exited successfully.");
                    } else {
                        throw new Exception($"Deployment exited with code: {deployProcess.ExitCode}");
                    }
                }

                // clean up
                File.Delete(ssfnDestination);

                if (File.Exists(ssfnBackup)) {
                    File.Move(ssfnBackup, ssfnDestination);
                }

                File.Delete(configDestination);

                if (File.Exists(configBackup)) {
                    File.Move(configBackup, configDestination);
                }
            }
        }
    }
}
