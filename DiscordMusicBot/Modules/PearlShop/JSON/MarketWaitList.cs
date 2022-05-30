using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordMusicBot.Modules.PearlShop
{
    public class MarketWaitList
    {
        #region Format
        public struct WaitItem
        {
            public int ItemID { get; set; }
            public int EnhancementLevel { get; set; }
            public long Price { get; set; }
            public long Timestamp { get; set; }

            public override string ToString()
            {
                return $"ItemID: {ItemID}, EnhancementLevel: {EnhancementLevel}, Price: {Price}, Timestamp: {Timestamp}";
            }
        }
        #endregion

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        [JsonProperty("resultMsg")]
        public string ResultMessage { get; set; }

        public List<WaitItem> GetItems()
        {
            List<WaitItem> items = new List<WaitItem>();
            if (!string.IsNullOrEmpty(ResultMessage))
            {
                string[] itemDatas = ResultMessage.Split("|");
                for (int i = 0; i < itemDatas.Length; i++)
                {
                    if (string.IsNullOrEmpty(itemDatas[i])) continue;

                    string[] itemData = itemDatas[i].Split("-");

                    //Parse
                    int.TryParse(itemData[0], out int itemID);
                    int.TryParse(itemData[1], out int enhancementLevel);
                    long.TryParse(itemData[2], out long price);
                    long.TryParse(itemData[3], out long timestamp);

                    //Apply
                    WaitItem item = new WaitItem
                    {
                        ItemID = itemID,
                        EnhancementLevel = enhancementLevel,
                        Price = price,
                        Timestamp = timestamp
                    };
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
