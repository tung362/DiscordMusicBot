using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace DiscordMusicBot
{
    public class DiscordCommandCategory
    {
        public string CategoryName { get; set; }

        /*Cache*/
        public List<DiscordCommandInfo> CommandInfos { get; private set; }

        public DiscordCommandCategory(string categoryName)
        {
            CategoryName = categoryName;
            CommandInfos = new List<DiscordCommandInfo>();
        }
    }
}
