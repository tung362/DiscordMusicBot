using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Modules.Music
{
    public class MusicCommandContext : DiscordCommandContext<MusicModule>
    {
        public MusicCommandContext(DiscordSocketClient client, IUserMessage msg, MusicModule module, CommandInteractionState interactionState = null) : base(client, msg, module, interactionState) { }
    }
}
