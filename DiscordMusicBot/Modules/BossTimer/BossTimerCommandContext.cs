using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Modules.BossTimer
{
    public class BossTimerCommandContext : DiscordCommandContext<BossTimerModule>
    {
        public BossTimerCommandContext(DiscordSocketClient client, IUserMessage msg, BossTimerModule module, CommandInteractionState interactionState = null) : base(client, msg, module, interactionState) { }
    }
}
