using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Modules.BossTimer
{
    public class BossTimerModule : DiscordModule
    {
        /*Cache*/

        public BossTimerModule(DiscordBotClient discordBot) : base(discordBot)
        {

        }

        protected override async Task OnServicesPreRegister(DiscordBotClient discordBot)
        {

        }

        protected override async Task OnServicesReady(DiscordBotClient discordBot, ServiceProvider serviceProvider)
        {

        }

        protected override async Task OnReady(DiscordBotClient discordBot)
        {

        }

        protected override async Task OnSlashCommandExecuted(DiscordBotClient discordBot, CommandInteractionState InteractionState)
        {
            BossTimerCommandContext context = new BossTimerCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

            IResult result = await DiscordBot.ClientCommandService.ExecuteAsync(context, InteractionState.Command, null);
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                //Command successful
            }
            else
            {
                //Command failed
            }
        }

        protected override async Task OnButtonExecuted(DiscordBotClient discordBot, CommandInteractionState InteractionState)
        {
            BossTimerCommandContext context = new BossTimerCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

            IResult result = await DiscordBot.ClientCommandService.ExecuteAsync(context, InteractionState.Command, null);
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                //Command successful
            }
            else
            {
                //Command failed
            }
        }
        protected override async Task OnSelectMenuExecuted(DiscordBotClient discordBot, CommandInteractionState InteractionState)
        {
            BossTimerCommandContext context = new BossTimerCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

            IResult result = await DiscordBot.ClientCommandService.ExecuteAsync(context, InteractionState.Command, null);
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                //Command successful
            }
            else
            {
                //Command failed
            }
        }

        /// <summary>
        /// Command handler hook
        /// </summary>
        /// <param name="socketMessage"></param>
        protected override async Task MessageReceivedHook(SocketMessage socketMessage)
        {
            SocketUserMessage userMessage = socketMessage as SocketUserMessage;
            if (userMessage == null) return;

            BossTimerCommandContext context = new BossTimerCommandContext(DiscordBot.Client, userMessage, this);

            //Filter
            if (!context.User.IsBot && !context.User.IsWebhook && !context.IsPrivate)
            {
                int argPos = 0;
                if (userMessage.HasStringPrefix(DiscordBot.Settings.CommandPrefix, ref argPos, StringComparison.OrdinalIgnoreCase) ||
                   userMessage.HasMentionPrefix(DiscordBot.Client.CurrentUser, ref argPos))
                {
                    IResult result = await DiscordBot.ClientCommandService.ExecuteAsync(context, argPos, null);

                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        //Command successful
                    }
                    else
                    {
                        //Command failed
                    }
                }
            }
        }

        protected override async Task JoinedGuildHook(SocketGuild guild)
        {

        }

        protected override async Task LeftGuildHook(SocketGuild guild)
        {

        }

        protected override async Task Disconnected(Exception exception)
        {

        }
    }
}
