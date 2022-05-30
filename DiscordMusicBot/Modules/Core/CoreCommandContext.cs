using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Modules.Core
{
    public class CoreCommandContext : DiscordCommandContext<CoreModule>
    {
        public CoreCommandContext(DiscordSocketClient client, IUserMessage msg, CoreModule module, CommandInteractionState interactionState = null) : base(client, msg, module, interactionState) { }
    }
}
