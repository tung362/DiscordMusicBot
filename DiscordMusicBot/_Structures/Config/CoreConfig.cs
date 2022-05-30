using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordMusicBot
{
    public class CoreConfig
    {
        public readonly string Token;
        public readonly string CommandPrefix;
        public readonly UserStatus ClientStatus;
        public readonly ActivityType ClientActivity;
        public readonly bool UseCoreModule;
        public readonly bool UseMusicModule;
        public readonly bool UseBossTimerModule;
        public readonly bool UsePearlShopModule;

        public CoreConfig(string path)
        {
            if (ConfigUtils.LoadConfig(path, out Dictionary<string, string> config))
            {
                //Parse
                int.TryParse(config["ClientStatus"], out int clientStatus);
                int.TryParse(config["ClientActivity"], out int clientActivity);
                bool.TryParse(config["CoreModule"], out bool useCoreModule);
                bool.TryParse(config["MusicModule"], out bool useMusicModule);
                bool.TryParse(config["BossTimerModule"], out bool useBossTimerModule);
                bool.TryParse(config["PearlShopModule"], out bool usePearlShopModule);

                //Apply
                Token = config["Token"];
                CommandPrefix = config["CommandPrefix"];
                ClientStatus = (UserStatus)clientStatus;
                ClientActivity = (ActivityType)clientActivity;
                UseCoreModule = useCoreModule;
                UseMusicModule = useMusicModule;
                UseBossTimerModule = useBossTimerModule;
                UsePearlShopModule = usePearlShopModule;
            }
        }
    }
}
