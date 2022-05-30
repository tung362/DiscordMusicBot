using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot.Modules.PearlShop
{
    public class PearlShopConfig
    {
        public readonly string MarketEndpoint;
        public readonly string MarketContentType;
        public readonly string MarketUserAgent;
        public readonly string SearchEndpoint;
        public readonly string SearchRegion;
        public readonly string SearchLanguage;

        public PearlShopConfig(string path)
        {
            if (ConfigUtils.LoadConfig(path, out Dictionary<string, string> config))
            {
                //Apply
                MarketEndpoint = config["MarketEndpoint"];
                MarketContentType = config["MarketContentType"];
                MarketUserAgent = config["MarketUserAgent"];
                SearchEndpoint = config["SearchEndpoint"];
                SearchRegion = config["SearchRegion"];
                SearchLanguage = config["SearchLanguage"];
            }
        }
    }
}
