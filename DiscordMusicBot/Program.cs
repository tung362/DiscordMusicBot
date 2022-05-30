using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using DiscordMusicBot.Modules.Core;
using DiscordMusicBot.Modules.Music;
using DiscordMusicBot.Modules.BossTimer;
using DiscordMusicBot.Modules.PearlShop;

namespace DiscordMusicBot
{
    /// <summary>
    /// Application entry point
    /// </summary>
    public class Program
    {
        public static DiscordBotClient DiscordBot;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        async Task Start()
        {
            string logPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Logs/ClientLog.txt";

            //Clear out log file
            await LogUtils.ClearLogAsync(logPath);

            //Create bot
            DiscordBot = new DiscordBotClient(logPath);

            //Load config
            CoreConfig config = new CoreConfig($"{AppDomain.CurrentDomain.BaseDirectory}/Configs/BotConfig.txt");
            DiscordBotSettings botSettings = ConfigureSettings(config, DiscordBot);

            //Setup bot
            await DiscordBot.Login(config.Token, botSettings);
            await ChangeCommandPrefix(DiscordBot, $"{botSettings.CommandPrefix}help");
            await ChangeActivity(DiscordBot, botSettings.ClientActivity);
            await ChangeStatus(DiscordBot, botSettings.ClientStatus);
            await DiscordBot.Init();

            //Set listeners
            DiscordBot.OnCommandPrefixChange += ChangeCommandPrefix;
            DiscordBot.OnStatusChange += ChangeStatus;
            DiscordBot.OnActivityChange += ChangeActivity;

            await Task.Delay(-1);
        }

        #region Settings
        DiscordBotSettings ConfigureSettings(CoreConfig config, DiscordBotClient discordBot)
        {
            DiscordBotSettings settings = new DiscordBotSettings
            {
                CommandPrefix = config.CommandPrefix,
                ClientStatus = config.ClientStatus,
                ClientActivity = config.ClientActivity
            };

            //Modules
            if (config.UseCoreModule) settings.ClientModules.AddSingleton(new CoreModule(discordBot));
            if (config.UseMusicModule) settings.ClientModules.AddSingleton(new MusicModule(discordBot));
            if (config.UseBossTimerModule) settings.ClientModules.AddSingleton(new BossTimerModule(discordBot));
            if (config.UsePearlShopModule) settings.ClientModules.AddSingleton(new PearlShopModule(discordBot));

            //Paths
            settings.Paths["yt-dlp"] = $"{AppDomain.CurrentDomain.BaseDirectory}/yt-dlp.exe";
            settings.Paths["ffmpeg"] = $"{AppDomain.CurrentDomain.BaseDirectory}/ffmpeg.exe";

            return settings;
        }
        #endregion

        #region Listeners
        async Task ChangeCommandPrefix(DiscordBotClient discordBot, string prefix)
        {
            await discordBot.Client.SetGameAsync($"{prefix}help", null, discordBot.Settings.ClientActivity);
        }

        async Task ChangeStatus(DiscordBotClient discordBot, UserStatus userStatus)
        {
            await discordBot.Client.SetStatusAsync(userStatus);
        }

        async Task ChangeActivity(DiscordBotClient discordBot, ActivityType activityType)
        {
            await discordBot.Client.SetGameAsync($"{discordBot.Settings.CommandPrefix}help", null, activityType);
        }
        #endregion
    }
}
