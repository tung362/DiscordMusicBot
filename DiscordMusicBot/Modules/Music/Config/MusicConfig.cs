using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot.Modules.Music
{
    public class MusicConfig
    {
        public readonly string LavalinkIP;
        public readonly ushort LavalinkPort;
        public readonly string LavalinkPassword;
        public readonly bool LavalinkSecureConnection;
        public readonly bool Log;

        public MusicConfig(string path)
        {
            if (ConfigUtils.LoadConfig(path, out Dictionary<string, string> config))
            {
                //Parse
                ushort.TryParse(config["LavalinkPort"], out ushort lavalinkPort);
                bool.TryParse(config["LavalinkSecureConnection"], out bool lavalinkSecureConnection);
                bool.TryParse(config["Log"], out bool log);

                //Apply
                LavalinkIP = config["LavalinkIP"];
                LavalinkPort = lavalinkPort;
                LavalinkPassword = config["LavalinkPassword"];
                LavalinkSecureConnection = lavalinkSecureConnection;
                Log = log;
            }
        }
    }
}
