using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Webhook;
using Discord.Commands;

namespace DiscordMusicBot
{
    public class CommandInteractionState
    {
        public enum StateType { SlashCommand, Button, SelectMenu, TextInput, ModalSubmit, ActionRow }

        public readonly SocketInteraction CommandInteraction;
        public readonly RestInteractionMessage Response;
        public readonly string Command;
        public readonly StateType State;

        public CommandInteractionState(SocketInteraction commandInteraction, RestInteractionMessage response, string command, StateType state)
        {
            CommandInteraction = commandInteraction;
            Response = response;
            Command = command;
            State = state;
        }
    }
}
