using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot
{
    public abstract class DiscordCommandContext<T> : CommandContext where T : DiscordModule
    {
        public T Module { get; private set; }
        public CommandInteractionState InteractionState { get; private set; }
        public DiscordCommandContext(DiscordSocketClient client, IUserMessage msg, T module, CommandInteractionState interactionState = null) : base(client, msg)
        {
            Module = module;
            InteractionState = interactionState;
        }
    }
}
