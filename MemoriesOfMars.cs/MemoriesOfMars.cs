using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class MemoriesOfMars : SteamCMDAgent
    {
        /* dedicated server guide https://505games.com/wp-content/uploads/Handbook-Dedicated-Servers-Memories-of-Mars-V1.5.pdf */

        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.MemoriesOfMars", // WindowsGSM.XXXX
            author = "ohmcodes",
            description = "WindowsGSM plugin for supporting Memories of Mars Dedicated Server",
            version = "1.0",
            url = "https://github.com/ohmcodes/WindowsGSM.MemoriesOfMars", // Github repository link (Best practice)
            color = "#f5bd1f" // Color Hex
        };

        // - Standard Constructor and properties
        public MemoriesOfMars(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string Error, Notice;

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "897590"; /* taken via https://steamdb.info/app/897590/info/ */

        // - Game server Fixed variables
        public override string StartPath => @"Game\Binaries\Win64\MemoriesOfMarsServer.exe"; // Game server start path
        public string FullName = "Memories of Mars Dedicated Server"; // Game server FullName
        public bool AllowsEmbedConsole = true;  // Does this server support output redirect?
        public int PortIncrements = 0; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = null; // Query method should be use on current server type. Accepted value: null or new A2S() or new FIVEM() or new UT3()

        // - Game server default values
        public string ServerName = "mom_dedicated";
        public string Defaultmap = "Main"; // Original (MapName)
        public string Maxplayers = "2"; // WGSM reads this as string but originally it is number or int (MaxPlayers)
        public string Port = "7777"; // WGSM reads this as string but originally it is number or int
        public string QueryPort = "27015"; // WGSM reads this as string but originally it is number or int (SteamQueryPort)
        public string Additional = ""; // Additional server start parameter


        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            // create new config
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"\t\"ServerName\": \"{_serverData.ServerName}\",");
            sb.AppendLine($"\t\"ServerPassword\": \"\",");
            sb.AppendLine($"\t\"ServerID\": \"mom_dedicated_01\",");
            sb.AppendLine($"\t\"MapName\": \"{_serverData.ServerMap}\",");
            sb.AppendLine($"\t\"MaxPlayers\": {_serverData.ServerMaxPlayer},");
            sb.AppendLine($"\t\"EnablePVP\": false,");
            sb.AppendLine($"\t\"EnablePVPAreas\": true,");
            sb.AppendLine($"\t\"EnableEAC\": true,");
            sb.AppendLine($"\t\"DailyRestartUTCHour\": \"12\",");
            sb.AppendLine($"\t\"Headless\": false,");
            sb.AppendLine($"\t\"UserWhitelist\": \"\",");
            sb.AppendLine($"\t\"UserBlacklist\": \"\",");
            sb.AppendLine($"\t\"Admins\": \"\"");
            sb.AppendLine("}");

            File.WriteAllText(ServerPath.GetServersServerFiles(_serverData.ServerID, "DedicatedServerConfig.cfg"), sb.ToString());
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {

            string shipExePath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);
            if (!File.Exists(shipExePath))
            {
                Error = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return null;
            }

            //Get WAN IP from net
            string externalIpString = new WebClient().DownloadString("http://ifconfig.me/ip").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            var externalIp = IPAddress.Parse(externalIpString);

            // Prepare start parameter
            var additional_param = new StringBuilder($"{_serverData.ServerParam}");

            string param = $"-MULTIHOME=\"{_serverData.ServerIP}\" -PublicIP=\"{externalIp}\" -port={_serverData.ServerPort} -beaconport={_serverData.ServerQueryPort} -maxplayers={_serverData.ServerMaxPlayer} ";
            
            param += additional_param;

            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = shipExePath,
                    Arguments = param.ToString(),
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            // Set up Redirect Input and Output to WindowsGSM Console if EmbedConsole is on
            if (AllowsEmbedConsole)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                var serverConsole = new ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;
            }

            // Start Process
            try
            {
                p.Start();
                if (AllowsEmbedConsole)
                {
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                }

                return p;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null; // return null if fail to start
            }
        }


        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                if (p.StartInfo.RedirectStandardInput)
                {
                    // Send "stop" command to StandardInput stream if EmbedConsole is on
                    p.StandardInput.WriteLine("stop");
                }
                else
                {
                    // Send "stop" command to game server process MainWindow
                    ServerConsole.SendMessageToMainWindow(p.MainWindowHandle, "stop");
                }
            });
        }

        // - Update server function
        public async Task<Process> Update(bool validate = false, string custom = null)
        {
            var (p, error) = await Installer.SteamCMD.UpdateEx(serverData.ServerID, AppId, validate, custom: custom, loginAnonymous: loginAnonymous);
            Error = error;
            await Task.Run(() => { p.WaitForExit(); });
            return p;
        }

    }
}
