using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordMusicBot.Modules.PearlShop
{
    public class ItemSearch
    {
        [JsonProperty("name")]
        public int Name { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("enhanceGrade")]
        public int EnhanceGrade { get; set; }

        [JsonProperty("totalTrades")]
        public int TotalTrades { get; set; }

        [JsonProperty("icon")]
        public int Icon { get; set; }
    }
}
