using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordMusicBot.Modules.PearlShop
{
    public class PearlShopModule : DiscordModule
    {
        /*Cache*/
        public HttpClient MarketClient { get; private set; }
        public HttpClient SearchClient { get; private set; }
        public PearlShopConfig Config { get; private set; }

        public PearlShopModule(DiscordBotClient discordBot) : base(discordBot)
        {
            //Init
            MarketClient = new HttpClient();
            SearchClient = new HttpClient();

            //Load module settings
            Config = new PearlShopConfig($"{AppDomain.CurrentDomain.BaseDirectory}/Configs/PearlShop.txt");

            //Register client headers
            MarketClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(Config.MarketContentType));
            MarketClient.DefaultRequestHeaders.Add("User-Agent", Config.MarketUserAgent);
        }

        protected override async Task OnServicesPreRegister(DiscordBotClient discordBot)
        {

        }

        protected override async Task OnServicesReady(DiscordBotClient discordBot, ServiceProvider serviceProvider)
        {

        }

        protected override async Task OnReady(DiscordBotClient discordBot)
        {
            HttpResponseMessage marketResponse = await MarketClient.PostAsync($"{Config.MarketEndpoint}/Trademarket/GetWorldMarketWaitList", null);
            string marketResponseContent = await marketResponse.Content.ReadAsStringAsync();
            MarketWaitList marketWaitList = JsonConvert.DeserializeObject<MarketWaitList>(marketResponseContent);

            List<MarketWaitList.WaitItem> items = marketWaitList.GetItems();

            for (int i = 0; i < items.Count; i++)
            {
                MarketWaitList.WaitItem item = items[i];
                Console.WriteLine(item.ToString());
                Console.WriteLine($"{Config.SearchEndpoint}/api/item-search/:{item.ItemID}?enhLevel={item.EnhancementLevel}&region={Config.SearchRegion}&lang={Config.SearchLanguage}");

                HttpResponseMessage searchResponse = await SearchClient.GetAsync($"{Config.SearchEndpoint}/api/item-search/{item.ItemID}?enhLevel={item.EnhancementLevel}&region={Config.SearchRegion}&lang={Config.SearchLanguage}");
                string searchResponseContent = await searchResponse.Content.ReadAsStringAsync();
                ItemSearch itemSearch = JsonConvert.DeserializeObject<ItemSearch>(searchResponseContent);

                Console.WriteLine(itemSearch.Name);
            }
        }

        protected override async Task OnSlashCommandExecuted(DiscordBotClient discordBot, CommandInteractionState InteractionState)
        {
            PearlShopCommandContext context = new PearlShopCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

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
            PearlShopCommandContext context = new PearlShopCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

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
            PearlShopCommandContext context = new PearlShopCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

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

            PearlShopCommandContext context = new PearlShopCommandContext(DiscordBot.Client, userMessage, this);

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

        #region Listeners

        #endregion
    }
}
