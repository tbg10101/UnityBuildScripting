using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

namespace Software10101.BuildScripting.Editor {
    public class DeployToSteamStep : AbstractBuildStep {
        [NotNull]
        public static string SteamCliPath = @"C:\Program Files\Steamworks SDK\tools\ContentBuilder\builder\steamcmd.exe";

        /// <summary>
        /// To get this file, log into Steam and enter your Steam Guard code like normal.
        /// A file with a name starting with "ssfn" be generated next to the Steam executable.
        /// https://partner.steamgames.com/doc/sdk/uploading
        /// </summary>
        [NotNull]
        public static string SsfnPath = @"SteamCreds\ssfn5073304987753677769";

        /// <summary>
        /// To get this file, log into Steam and enter your Steam Guard code like normal.
        /// A file named "config.vdf" will be generated in a directory called "config" next to the Steam executable.
        /// https://partner.steamgames.com/doc/sdk/uploading
        /// </summary>
        [NotNull]
        public static string ConfigVdfPath = @"SteamCreds\config\config.vdf";

        private readonly uint _appId;
        private readonly uint _depotId;
        private readonly string _contentRoot;
        private readonly string _accountName;
        private readonly string _accountPassword;

        public DeployToSteamStep(
            uint appId,
            uint depotId,
            string contentRoot,
            string accountName,
            string accountPassword
        ) {
            _appId = appId;
            _depotId = depotId;
            _contentRoot = contentRoot;
            _accountName = accountName;
            _accountPassword = accountPassword;
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
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
            string ssfnDestination = Path.GetFullPath(Path.Combine(SteamCliPath, "..", Path.GetFileName(SsfnPath)));
            string ssfnBackup = $"{ssfnDestination}.bak";

            if (File.Exists(ssfnDestination)) {
                if (File.Exists(ssfnBackup)) {
                    File.Delete(ssfnBackup);
                }

                File.Move(ssfnDestination, ssfnBackup);
            }

            File.Copy(SsfnPath, ssfnDestination);

            string configParent = Path.GetFullPath(Path.Combine(SteamCliPath, "..", "config"));

            if (!Directory.Exists(configParent)) {
                Directory.CreateDirectory(configParent);
            }

            string configDestination = Path.Combine(configParent, Path.GetFileName(ConfigVdfPath));
            string configBackup = $"{configDestination}.bak";

            if (File.Exists(configDestination)) {
                if (File.Exists(configBackup)) {
                    File.Delete(configBackup);
                }

                File.Move(configDestination, configBackup);
            }

            File.Copy(ConfigVdfPath, configDestination);

            // execute deployment
            string args = $"+@ShutdownOnFailedCommand 1 +@NoPromptForPassword 1 +login \"{_accountName}\" \"{_accountPassword}\" +run_app_build \"{vdfFullPath}\" +quit";
            Debug.Log($"Beginning deployment: {SteamCliPath} {args}");

            using (Process deployProcess = new Process {
                       StartInfo = new ProcessStartInfo {
                           FileName = SteamCliPath,
                           Arguments = args,
                           CreateNoWindow = true,
                           UseShellExecute = false,
                           RedirectStandardOutput = true,
                           RedirectStandardError = true
                       }
                    }
            ) {
                deployProcess.OutputDataReceived += (_,  message) => {
                    if (string.IsNullOrEmpty(message.Data))
                    {
                        return;
                    }

                    Debug.Log(message.Data);
                };

                deployProcess.ErrorDataReceived += (_, message) => {
                    if (string.IsNullOrEmpty(message.Data))
                    {
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
