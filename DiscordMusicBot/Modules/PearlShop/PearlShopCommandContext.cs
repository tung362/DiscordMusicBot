using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Modules.PearlShop
{
    public class PearlShopCommandContext : DiscordCommandContext<PearlShopModule>
    {
        public PearlShopCommandContext(DiscordSocketClient client, IUserMessage msg, PearlShopModule module, CommandInteractionState interactionState = null) : base(client, msg, module, interactionState) { }
    }
}
