using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot
{
    public abstract class DiscordModule
    {
        public readonly DiscordBotClient DiscordBot;

        public DiscordModule(DiscordBotClient discordBot)
        {
            DiscordBot = discordBot;

            //Set listeners
            discordBot.OnServicesPreRegister += OnServicesPreRegister;
            discordBot.OnServicesReady += OnServicesReady;
            discordBot.OnReady += OnReady;
            discordBot.OnSlashCommandExecuted += OnSlashCommandExecuted;
            discordBot.OnButtonExecuted += OnButtonExecuted;
            discordBot.OnSelectMenuExecuted += OnSelectMenuExecuted;
            discordBot.Client.MessageReceived += MessageReceivedHook;
            discordBot.Client.JoinedGuild += JoinedGuildHook;
            discordBot.Client.LeftGuild += LeftGuildHook;
            discordBot.Client.Disconnected += Disconnected;
        }

        #region Listeners
        protected virtual async Task OnServicesPreRegister(DiscordBotClient discordBot) { }
        protected virtual async Task OnServicesReady(DiscordBotClient discordBot, ServiceProvider serviceProvider) { }
        protected virtual async Task OnReady(DiscordBotClient discordBot) { }
        protected virtual async Task OnSlashCommandExecuted(DiscordBotClient discordBot, CommandInteractionState commandInteractionState) { }
        protected virtual async Task OnButtonExecuted(DiscordBotClient discordBot, CommandInteractionState commandInteractionState) { }
        protected virtual async Task OnSelectMenuExecuted(DiscordBotClient discordBot, CommandInteractionState commandInteractionState) { }
        /// <summary>
        /// Command handler hook
        /// </summary>
        /// <param name="socketMessage"></param>
        protected virtual async Task MessageReceivedHook(SocketMessage socketMessage) { }
        protected virtual async Task JoinedGuildHook(SocketGuild guild) { }
        protected virtual async Task LeftGuildHook(SocketGuild guild) { }
        protected virtual async Task Disconnected(Exception exception) { }
        #endregion
    }
}
