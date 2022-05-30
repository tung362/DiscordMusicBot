using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Webhook;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot
{
    /// <summary>
    /// Represents a discord bot instance
    /// </summary>
    public class DiscordBotClient
    {
        /*Clients*/
        public DiscordSocketClient Client { get; private set; }
        public Dictionary<ulong, DiscordWebhookClient> ClientWebhooks { get; private set; }

        /*Services*/
        public CommandService ClientCommandService { get; private set; }

        /*Settings*/
        public DiscordBotSettings Settings { get; private set; }

        /*Commands*/
        public Dictionary<string, SlashCommandBuilder> GlobalSlashCommands { get; private set; }
        public Dictionary<string, SlashCommandBuilder> GuildSlashCommands { get; private set; }
        private List<DiscordCommandCategory> CommandCategories { get; }
        private Dictionary<string, DiscordCommandInfo> CommandInfos { get; }

        /*Events*/
        public event Func<DiscordBotClient, Task> OnLogin;
        public event Func<DiscordBotClient, Task> OnServicesPreRegister;
        public event Func<DiscordBotClient, ServiceProvider, Task> OnServicesReady;
        public event Func<DiscordBotClient, Task> OnReady;
        public event Func<DiscordBotClient, Task> OnLogout;
        public event Func<DiscordBotClient, CommandInteractionState, Task> OnSlashCommandExecuted;
        public event Func<DiscordBotClient, CommandInteractionState, Task> OnButtonExecuted;
        public event Func<DiscordBotClient, CommandInteractionState, Task> OnSelectMenuExecuted;
        public event Func<DiscordBotClient, string, Task> OnCommandPrefixChange;
        public event Func<DiscordBotClient, UserStatus, Task> OnStatusChange;
        public event Func<DiscordBotClient, ActivityType, Task> OnActivityChange;

        /*Cache*/
        public bool Ready { get; private set; }
        public readonly string LogPath;

        /*Properties*/
        public int CommandCategoriesCount { get { return CommandCategories.Count; } }
        public int CommandInfosCount { get { return CommandInfos.Count; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public DiscordBotClient(string logPath = null)
        {
            //Init
            Client = new DiscordSocketClient();
            ClientWebhooks = new Dictionary<ulong, DiscordWebhookClient>();
            ClientCommandService = new CommandService();
            GlobalSlashCommands = new Dictionary<string, SlashCommandBuilder>();
            GuildSlashCommands = new Dictionary<string, SlashCommandBuilder>();
            CommandCategories = new List<DiscordCommandCategory>();
            CommandInfos = new Dictionary<string, DiscordCommandInfo>();
            LogPath = logPath;
        }

        /// <summary>
        /// Login bot and webhooks to discord
        /// </summary>
        public async Task Login(string token, DiscordBotSettings settings)
        {
            //Init
            Settings = settings;

            //Bot login
            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            //Log
            Client.Log += (log) => LogUtils.LogAsync($"Client: {log.ToString()}", LogPath);

            OnLogin?.Invoke(this);
        }

        public async Task LoginWebhook(ulong id, string token)
        {
            DiscordWebhookClient webhook = new DiscordWebhookClient(id, token);
            ClientWebhooks.Add(id, webhook);

            webhook.Log += (log) => LogUtils.LogAsync($"Webhook {id}: {log.ToString()}", LogPath);
        }

        /// <summary>
        /// Initiate bot behaviors
        /// </summary>
        public async Task Init()
        {
            //Register modules
            OnServicesPreRegister?.Invoke(this);
            ServiceProvider clientServices = Settings.ClientModules.BuildServiceProvider();
            await ClientCommandService.AddModulesAsync(Assembly.GetEntryAssembly(), clientServices);
            OnServicesReady?.Invoke(this, clientServices);

            //Set listeners
            Client.Ready += ClientReady;
            Client.SlashCommandExecuted += SlashCommandExecuted;
            Client.ButtonExecuted += ButtonExecuted;
            Client.SelectMenuExecuted += SelectMenuExecuted;
            Settings.OnCommandPrefixChange += CommandPrefixChange;
            Settings.OnStatusChange += StatusChange;
            Settings.OnActivityChange += ActivityChange;

            //Dispose
            await clientServices.DisposeAsync();
        }

        public async Task Logout()
        {
            await Client.LogoutAsync();
            OnLogout?.Invoke(this);
        }

        #region Listeners
        /// <summary>
        /// Client ready hook
        /// </summary>
        /// <returns></returns>
        async Task ClientReady()
        {
            Ready = true;
            OnReady?.Invoke(this);
        }

        async Task SlashCommandExecuted(SocketSlashCommand slashCommand)
        {
            await slashCommand.RespondAsync("Command received.");

            RestInteractionMessage response = await slashCommand.GetOriginalResponseAsync();
            if (response == null) return;

            string command = $"{slashCommand.CommandName}";
            SocketSlashCommandDataOption[] options = slashCommand.Data.Options.ToArray();
            for (int i = 0; i < options.Length; i++) command += $" {options[i].Value}";
            OnSlashCommandExecuted?.Invoke(this, new CommandInteractionState(slashCommand, response, command, CommandInteractionState.StateType.SlashCommand));
        }

        async Task ButtonExecuted(SocketMessageComponent messageComponent)
        {
            await messageComponent.DeferAsync();

            RestInteractionMessage response = await messageComponent.GetOriginalResponseAsync();
            if (response == null) return;

            string command = $"{messageComponent.Data.CustomId}";
            OnButtonExecuted?.Invoke(this, new CommandInteractionState(messageComponent, response, command, CommandInteractionState.StateType.Button));
        }

        async Task SelectMenuExecuted(SocketMessageComponent messageComponent)
        {
            await messageComponent.DeferAsync();

            RestInteractionMessage response = await messageComponent.GetOriginalResponseAsync();
            if (response == null) return;

            string command = $"{messageComponent.Data.CustomId} {string.Join(", ", messageComponent.Data.Values)}";
            OnSelectMenuExecuted?.Invoke(this, new CommandInteractionState(messageComponent, response, command, CommandInteractionState.StateType.SelectMenu));
        }

        void CommandPrefixChange(string prefix)
        {
            OnCommandPrefixChange?.Invoke(this, prefix);
        }

        void StatusChange(UserStatus status)
        {
            OnStatusChange?.Invoke(this, status);
        }

        void ActivityChange(ActivityType activity)
        {
            OnActivityChange?.Invoke(this, activity);
        }
        #endregion

        #region Utils
        public void AddCommandCategory(DiscordCommandCategory commandCategory)
        {
            for (int i = 0; i < commandCategory.CommandInfos.Count; i++)
            {
                DiscordCommandInfo commandInfo = commandCategory.CommandInfos[i];
                if (CommandInfos.ContainsKey(commandInfo.CommandName)) LogUtils.Log($"Client: Warning! Duplicate command info \"{commandInfo.CommandName}\"", LogPath);
                CommandInfos[commandInfo.CommandName] = commandInfo;
            }
            CommandCategories.Add(commandCategory);
        }

        public void RemoveCommandCategory(int index)
        {
            for (int i = 0; i < CommandCategories[index].CommandInfos.Count; i++)
            {
                DiscordCommandInfo commandInfo = CommandCategories[index].CommandInfos[i];
                if (CommandInfos.ContainsKey(commandInfo.CommandName)) CommandInfos.Remove(commandInfo.CommandName);
            }
            CommandCategories.RemoveAt(index);
        }

        public DiscordCommandCategory GetCommandCategory(int index)
        {
            return CommandCategories[index];
        }

        public bool TryGetCommandInfo(string commandName, out DiscordCommandInfo commandInfo)
        {
            if (CommandInfos.ContainsKey(commandName))
            {
                commandInfo = CommandInfos[commandName];
                return true;
            }
            commandInfo = null;
            return false;
        }
        #endregion
    }
}
