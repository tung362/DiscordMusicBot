using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordMusicBot.Modules.Music
{
    public class MusicModule : DiscordModule
    {
        /*Cache*/
        public LavaNode LavaNodeService { get; private set; }
        public LavaConfig LavaConfigService { get; private set; }
        public Dictionary<ulong, GuildMusicState> GuildMusicStates { get; private set; }
        public MusicConfig Config { get; private set; }

        public MusicModule(DiscordBotClient discordBot) : base(discordBot)
        {
            //Init
            GuildMusicStates = new Dictionary<ulong, GuildMusicState>();

            //Load module settings
            Config = new MusicConfig($"{AppDomain.CurrentDomain.BaseDirectory}/Configs/MusicConfig.txt");

            //Register module command infos
            DiscordCommandCategory commandCategory = new DiscordCommandCategory("Music");
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/connect.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/disconnect.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/play.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/skip.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/seek.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/loop.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/pause.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/resume.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/stop.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/volume.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/playlist.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/removetrack.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/clearplaylist.txt"));
            commandCategory.CommandInfos.Add(new DiscordCommandInfo($"{AppDomain.CurrentDomain.BaseDirectory}/Localization/Music/lyrics.txt"));
            discordBot.AddCommandCategory(commandCategory);

            //Register module slash commands
            SlashCommandBuilder connectSlashCommand = SlashCommandUtils.CreateSlashCommand("connect", "Join voice chat the user is in.");
            discordBot.GuildSlashCommands.Add(connectSlashCommand.Name, connectSlashCommand);

            SlashCommandBuilder disconnectSlashCommand = SlashCommandUtils.CreateSlashCommand("disconnect", "Leave the currently connected voice chat.");
            discordBot.GuildSlashCommands.Add(disconnectSlashCommand.Name, disconnectSlashCommand);

            SlashCommandBuilder playSlashCommand = SlashCommandUtils.CreateSlashCommand("play", "Searches and plays a music track from Youtube, Soundcloud, or from a direct link.");
            playSlashCommand.AddOption(SlashCommandUtils.CreateSlashCommandOption("search", "Song name or direct link", ApplicationCommandOptionType.String));
            discordBot.GuildSlashCommands.Add(playSlashCommand.Name, playSlashCommand);

            SlashCommandBuilder skipSlashCommand = SlashCommandUtils.CreateSlashCommand("skip", "Skip to the next music track within the playlist.");
            discordBot.GuildSlashCommands.Add(skipSlashCommand.Name, skipSlashCommand);

            SlashCommandBuilder seekSlashCommand = SlashCommandUtils.CreateSlashCommand("seek", "Skip to the next music track within the playlist.");
            seekSlashCommand.AddOption(SlashCommandUtils.CreateSlashCommandOption("index", "Position in the playlist to seek to", ApplicationCommandOptionType.Integer));
            discordBot.GuildSlashCommands.Add(seekSlashCommand.Name, seekSlashCommand);

            SlashCommandBuilder loopSlashCommand = SlashCommandUtils.CreateSlashCommand("loop", "Calling the command multiple times will switch from loop track, loop playlist, disable looping.");
            discordBot.GuildSlashCommands.Add(loopSlashCommand.Name, loopSlashCommand);

            SlashCommandBuilder pauseSlashCommand = SlashCommandUtils.CreateSlashCommand("pause", "Pause the currently playing music track.");
            discordBot.GuildSlashCommands.Add(pauseSlashCommand.Name, pauseSlashCommand);

            SlashCommandBuilder resumeSlashCommand = SlashCommandUtils.CreateSlashCommand("resume", "Resume the currently paused music track.");
            discordBot.GuildSlashCommands.Add(resumeSlashCommand.Name, resumeSlashCommand);

            SlashCommandBuilder stopSlashCommand = SlashCommandUtils.CreateSlashCommand("stop", "Stop the currently playing music track, clears the playlist, and reset the music player.");
            discordBot.GuildSlashCommands.Add(stopSlashCommand.Name, stopSlashCommand);

            SlashCommandBuilder volumeSlashCommand = SlashCommandUtils.CreateSlashCommand("volume", "Changes the volume of the music player from range of 0 - 200.");
            volumeSlashCommand.AddOption(SlashCommandUtils.CreateSlashCommandOption("volume", "Volume of the music player from range of 0 - 200", ApplicationCommandOptionType.Integer));
            discordBot.GuildSlashCommands.Add(volumeSlashCommand.Name, volumeSlashCommand);

            SlashCommandBuilder playlistSlashCommand = SlashCommandUtils.CreateSlashCommand("playlist", "Displays all music tracks queued.");
            discordBot.GuildSlashCommands.Add(playlistSlashCommand.Name, playlistSlashCommand);

            SlashCommandBuilder removetrackSlashCommand = SlashCommandUtils.CreateSlashCommand("removetrack", "Remove music track at the specified position within the playlist.");
            removetrackSlashCommand.AddOption(SlashCommandUtils.CreateSlashCommandOption("index", "The music track position to remove in the playlist", ApplicationCommandOptionType.Integer));
            discordBot.GuildSlashCommands.Add(removetrackSlashCommand.Name, removetrackSlashCommand);

            SlashCommandBuilder clearplaylistSlashCommand = SlashCommandUtils.CreateSlashCommand("clearplaylist", "Clear all music tracks queued.");
            discordBot.GuildSlashCommands.Add(clearplaylistSlashCommand.Name, clearplaylistSlashCommand);

            SlashCommandBuilder lyricsSlashCommand = SlashCommandUtils.CreateSlashCommand("lyrics", "Displays lyrics of the currently playing music track.");
            discordBot.GuildSlashCommands.Add(lyricsSlashCommand.Name, lyricsSlashCommand);
        }

        protected override async Task OnServicesPreRegister(DiscordBotClient discordBot)
        {
            //Load config
            LavaConfig lavalinkNodeOptions = new LavaConfig
            {
                Hostname = Config.LavalinkIP,
                Port = Config.LavalinkPort,
                Authorization = Config.LavalinkPassword,
                IsSsl = Config.LavalinkSecureConnection
            };

            discordBot.Settings.ClientModules.AddSingleton(new LavaNode(discordBot.Client, lavalinkNodeOptions));
            discordBot.Settings.ClientModules.AddSingleton(lavalinkNodeOptions);
        }

        protected override async Task OnServicesReady(DiscordBotClient discordBot, ServiceProvider serviceProvider)
        {
            LavaNodeService = serviceProvider.GetRequiredService<LavaNode>();
            LavaConfigService = serviceProvider.GetRequiredService<LavaConfig>();
        }

        protected override async Task OnReady(DiscordBotClient discordBot)
        {
            //Guild registration
            List<SocketGuild> guilds = discordBot.Client.Guilds.ToList();
            for (int i = 0; i < guilds.Count; i++) GuildMusicStates[guilds[i].Id] = new GuildMusicState();

            if (!LavaNodeService.IsConnected)
            {
                if(Config.Log) LavaNodeService.OnLog += (log) => LogUtils.LogAsync($"Client: {log.ToString()}", discordBot.LogPath);
                await LavaNodeService.ConnectAsync();
                LavaNodeService.OnTrackEnded += OnTrackEnded;
                LavaNodeService.OnWebSocketClosed += OnWebSocketClosed;
            }
        }

        protected override async Task OnSlashCommandExecuted(DiscordBotClient discordBot, CommandInteractionState InteractionState)
        {
            MusicCommandContext context = new MusicCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

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
            MusicCommandContext context = new MusicCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

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
            MusicCommandContext context = new MusicCommandContext(DiscordBot.Client, InteractionState.Response, this, InteractionState);

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

            MusicCommandContext context = new MusicCommandContext(DiscordBot.Client, userMessage, this);

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
            GuildMusicStates[guild.Id] = new GuildMusicState();
        }

        protected override async Task LeftGuildHook(SocketGuild guild)
        {
            if (GuildMusicStates.ContainsKey(guild.Id)) GuildMusicStates.Remove(guild.Id);
        }

        protected override async Task Disconnected(Exception exception)
        {

        }

        #region Listeners
        async Task OnTrackEnded(TrackEndedEventArgs arg)
        {
            GuildMusicState guildMusicState = GuildMusicStates[arg.Player.TextChannel.GuildId];
            int seekOffset = 0;

            if (arg.Reason == TrackEndReason.Stopped && !guildMusicState.PendingIgnoreStop) return;

            guildMusicState.PendingIgnoreStop = false;
            await guildMusicState.DeleteMessage("play");
            await guildMusicState.DeleteMessage("skip");
            await guildMusicState.DeleteMessage("seek");
            await guildMusicState.DeleteMessage("pause");
            await guildMusicState.DeleteMessage("resume");
            await guildMusicState.DeleteMessage("stop");

            if (guildMusicState.LoopState == GuildMusicState.LoopType.LoopTrack)
            {
                await arg.Player.PlayAsync(guildMusicState.CurrentTrack);
                return;
            }
            else if (guildMusicState.LoopState == GuildMusicState.LoopType.None)
            {
                guildMusicState.RemoveTrack(guildMusicState.PlaylistPos);
                seekOffset--;
            }

            if (guildMusicState.PendingSeek.Item1)
            {
                guildMusicState.Seek(guildMusicState.PendingSeek.Item2 + seekOffset);
                guildMusicState.PendingSeek = Tuple.Create(false, -1);
            }
            else guildMusicState.Forward();

            if (guildMusicState.PlaylistPos >= 0)
            {
                await arg.Player.PlayAsync(guildMusicState.CurrentTrack);
                ComponentBuilder cb = new ComponentBuilder();
                cb.WithButton("Pause & Resume", "pausetoggle", ButtonStyle.Success);
                cb.WithButton(null, "volumedown", ButtonStyle.Primary, new Emoji("🔈"));
                cb.WithButton(null, "volumeup", ButtonStyle.Primary, new Emoji("🔊"));
                cb.WithButton("Skip", "silentskip", ButtonStyle.Primary);
                cb.WithButton("Stop", "stop", ButtonStyle.Danger);
                cb.WithButton("Playlist", "playlist", ButtonStyle.Secondary);
                guildMusicState.Messages["play"] = await arg.Player.TextChannel.SendNormalEmbedAsync(ColorUtils.MainColor, $"[{guildMusicState.CurrentTrack.Title}]({guildMusicState.CurrentTrack.Url})", "🎶 Now Playing", $"Duration: [{guildMusicState.CurrentTrack.Duration}]", await guildMusicState.CurrentTrack.FetchArtworkAsync(), cb.Build());
            }
        }

        async Task OnWebSocketClosed(WebSocketClosedEventArgs arg)
        {
            SocketGuild guild = DiscordBot.Client.GetGuild(arg.GuildId);
            if (guild == null) return;

            if (LavaNodeService.TryGetPlayer(guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = GuildMusicStates[arg.GuildId];

                await LavaNodeService.LeaveAsync(lavaPlayer.VoiceChannel);
                await guildMusicState.DeleteMessage("play");
                await guildMusicState.DeleteMessage("skip");
                await guildMusicState.DeleteMessage("seek");
                await guildMusicState.DeleteMessage("pause");
                await guildMusicState.DeleteMessage("resume");
                await guildMusicState.DeleteMessage("stop");
                await guildMusicState.Reset();
                return;
            }
        }
        #endregion
    }
}
