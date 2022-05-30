using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Audio;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace DiscordMusicBot.Modules.Music
{
    /// <summary>
    /// Handles music commands
    /// </summary>
    public class MusicCommands : ModuleBase<MusicCommandContext>
    {
        #region Commands
        [Command("connect", true, RunMode = RunMode.Async)]
        public async Task Connect(bool isPlay = false)
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            EmbedBuilder eb = new EmbedBuilder();
            IVoiceState userVoiceState = user as IVoiceState;
            GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

            if (userVoiceState.VoiceChannel == null)
            {
                if(!Context.Module.LavaNodeService.HasPlayer(Context.Guild)) await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | You must be in a voice channel.");
                return;
            }

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer) && !isPlay)
            {
                if (userVoiceState.VoiceChannel.Id == lavaPlayer.VoiceChannel.Id)
                {
                    await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Already in voice channel.");
                    return;
                }
                else
                {
                    await Context.Module.LavaNodeService.LeaveAsync(lavaPlayer.VoiceChannel);
                    await guildMusicState.Reset();
                }
            }

            try
            {
                await Context.Module.LavaNodeService.JoinAsync(userVoiceState.VoiceChannel, Context.Channel as ITextChannel);
                if (!isPlay) await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"{EmojiUtils.BotIcon} | Joined { userVoiceState.VoiceChannel.Name }.");
            }
            catch (Exception exception)
            {
                await Context.Channel.SendErrorEmbedAsync(exception.Message);
            }
        }

        [Command("disconnect", RunMode = RunMode.Async)]
        public async Task Disconnect()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                await Context.Module.LavaNodeService.LeaveAsync(lavaPlayer.VoiceChannel);
                await guildMusicState.Reset();
                await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"{user.Username} | Bye bye!");
                return;
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing playing in this server.");
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string search)
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            await Connect(true);
            if (!Context.Module.LavaNodeService.HasPlayer(Context.Guild)) return;

            if (string.IsNullOrWhiteSpace(search))
            {
                await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | The search cannot be empty.");
                return;
            }

            //Search
            SearchResponse searchResponse;
            if (search.Contains(".com")) searchResponse = await Context.Module.LavaNodeService.SearchAsync(SearchType.Direct, search);
            else searchResponse = await Context.Module.LavaNodeService.SearchYouTubeAsync(search);

            if (searchResponse.Status == SearchStatus.LoadFailed || searchResponse.Status == SearchStatus.NoMatches)
            {
                await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | No matches for \"{search}\".");
                return;
            }

            //Add to playlist and play
            LavaPlayer lavaPlayer = Context.Module.LavaNodeService.GetPlayer(Context.Guild);
            GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
            List<LavaTrack> tracks = searchResponse.Tracks.ToList();
            LavaTrack track = tracks[0];

            if (lavaPlayer.PlayerState == PlayerState.Playing || lavaPlayer.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    guildMusicState.Playlist.AddRange(searchResponse.Tracks);
                    await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Added {searchResponse.Tracks.Count} tracks to the playlist.", "Playlist");
                }
                else
                {
                    guildMusicState.Playlist.Add(track);
                    await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Added {track.Title} to the playlist.", "Playlist");
                }
                return;
            }

            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                guildMusicState.Playlist.AddRange(searchResponse.Tracks);
                await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Added {searchResponse.Tracks.Count} tracks to the playlist.", "Playlist");
            }
            else guildMusicState.Playlist.Add(track);

            guildMusicState.Forward();
            await lavaPlayer.PlayAsync(guildMusicState.CurrentTrack);
            await guildMusicState.DeleteMessage("play");
            await guildMusicState.DeleteMessage("skip");
            await guildMusicState.DeleteMessage("seek");
            await guildMusicState.DeleteMessage("pause");
            await guildMusicState.DeleteMessage("resume");
            await guildMusicState.DeleteMessage("stop");

            ComponentBuilder cb = new ComponentBuilder();
            cb.WithButton("Pause & Resume", "pausetoggle", ButtonStyle.Success);
            cb.WithButton(null, "volumedown", ButtonStyle.Primary, new Emoji("🔈"));
            cb.WithButton(null, "volumeup", ButtonStyle.Primary, new Emoji("🔊"));
            cb.WithButton("Skip", "silentskip", ButtonStyle.Primary);
            cb.WithButton("Stop", "stop", ButtonStyle.Danger);
            cb.WithButton("Playlist", "playlist", ButtonStyle.Secondary);
            guildMusicState.Messages["play"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"[{guildMusicState.CurrentTrack.Title}]({guildMusicState.CurrentTrack.Url})", "🎶 Now Playing", $"Duration: [{guildMusicState.CurrentTrack.Duration}]", await guildMusicState.CurrentTrack.FetchArtworkAsync(), cb.Build());
        }

        [Command("skip", true, RunMode = RunMode.Async)]
        public async Task Skip()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                if (lavaPlayer.PlayerState == PlayerState.Playing || lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                    guildMusicState.PendingIgnoreStop = true;
                    await lavaPlayer.StopAsync();
                    await guildMusicState.DeleteMessage("skip");
                    guildMusicState.Messages["skip"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Skipping music track.");
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing to skip in this server.");
        }

        [Command("seek", RunMode = RunMode.Async)]
        public async Task Seek(int index)
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                if (lavaPlayer.PlayerState == PlayerState.Playing || lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                    guildMusicState.PendingIgnoreStop = true;
                    guildMusicState.PendingSeek = Tuple.Create(true, index);
                    await lavaPlayer.StopAsync();
                    await guildMusicState.DeleteMessage("seek");
                    guildMusicState.Messages["seek"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Seeking music track at position: {index}.");
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing to seek in this server.");
        }

        [Command("loop", RunMode = RunMode.Async)]
        public async Task Loop()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                await guildMusicState.DeleteMessage("loop");
                switch (guildMusicState.LoopState)
                {
                    case GuildMusicState.LoopType.None:
                        guildMusicState.LoopState = GuildMusicState.LoopType.LoopTrack;
                        guildMusicState.Messages["loop"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Now looping track.");
                        break;
                    case GuildMusicState.LoopType.LoopTrack:
                        guildMusicState.LoopState = GuildMusicState.LoopType.LoopPlaylist;
                        guildMusicState.Messages["loop"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Now looping playlist.");
                        break;
                    case GuildMusicState.LoopType.LoopPlaylist:
                        guildMusicState.LoopState = GuildMusicState.LoopType.None;
                        guildMusicState.Messages["loop"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Now no longer looping.");
                        break;
                    default:
                        break;
                }
                return;
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing to loop in this server.");
        }

        [Command("pause", RunMode = RunMode.Async)]
        public async Task Pause()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                if (lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Music track already paused.");
                    return;
                }

                if (lavaPlayer.PlayerState == PlayerState.Playing)
                {
                    await lavaPlayer.PauseAsync();
                    await guildMusicState.DeleteMessage("pause");
                    await guildMusicState.DeleteMessage("resume");
                    await guildMusicState.DeleteMessage("stop");
                    //await guildMusicState.DeleteMessage("play");
                    guildMusicState.Messages["pause"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Music track paused.");
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing to pause in this server.");
        }

        [Command("resume", RunMode = RunMode.Async)]
        public async Task Resume()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                if (lavaPlayer.PlayerState == PlayerState.Playing)
                {
                    await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Music track already resumed.");
                    return;
                }

                if (lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    await lavaPlayer.ResumeAsync();
                    await guildMusicState.DeleteMessage("resume");
                    //await guildMusicState.DeleteMessage("play");
                    await guildMusicState.DeleteMessage("pause");
                    await guildMusicState.DeleteMessage("stop");
                    guildMusicState.Messages["resume"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Music track resumed.");
                    //guildMusicState.Messages["play"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"[{guildMusicState.CurrentTrack.Title}]({guildMusicState.CurrentTrack.Url})", "🎶 Now Playing", $"Duration: [{guildMusicState.CurrentTrack.Duration}]", await guildMusicState.CurrentTrack.FetchArtworkAsync());
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing to resume in this server.");
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                if (lavaPlayer.PlayerState == PlayerState.Stopped)
                {
                    await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Music track already stopped.");
                    return;
                }

                if (lavaPlayer.PlayerState == PlayerState.Playing)
                {
                    GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                    await lavaPlayer.StopAsync();
                    await guildMusicState.Reset();
                    await guildMusicState.DeleteMessage("stop");
                    await guildMusicState.DeleteMessage("resume");
                    await guildMusicState.DeleteMessage("pause");
                    await guildMusicState.DeleteMessage("play");
                    guildMusicState.Messages["stop"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Music track stopped.");
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing playing in this server.");
        }

        [Command("volume", RunMode = RunMode.Async)]
        public async Task Volume(int volume)
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                guildMusicState.Volume = Math.Clamp(volume, 0, 200);
                await lavaPlayer.UpdateVolumeAsync((ushort)guildMusicState.Volume);
                await guildMusicState.DeleteMessage("volume");
                guildMusicState.Messages["volume"] = await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Volume changed to {guildMusicState.Volume}%.");
                return;
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing playing in this server to change volume.");
        }

        [Command("playlist", RunMode = RunMode.Async)]
        public async Task Playlist()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                if (guildMusicState.Playlist.Count <= 0)
                {
                    await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | There's nothing in the playlist");
                    return;
                }

                UnicodeEncoding encoding = new UnicodeEncoding();
                using (MemoryStream ms = new MemoryStream())
                {
                    StreamWriter sw = new StreamWriter(ms, encoding);
                    try
                    {
                        for (int i = 0; i < guildMusicState.Playlist.Count; i++) await sw.WriteLineAsync($"{i}. {guildMusicState.Playlist[i].Title}");
                        await sw.FlushAsync();
                        ms.Seek(0, SeekOrigin.Begin);
                        await Context.Channel.SendFileAsync(ms, "Playlist.txt");
                    }
                    finally
                    {
                        await sw.DisposeAsync();
                    }
                    ms.Close();
                }
                return;
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing playing in this server to get playlist.");
        }

        [Command("removetrack", RunMode = RunMode.Async)]
        public async Task RemoveTrack(int index)
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                if (lavaPlayer.PlayerState == PlayerState.Playing || lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                    if (guildMusicState.RemoveTrack(index)) await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Track at index: {index} removed.");
                    else await Context.Channel.SendErrorEmbedAsync($"Track at index: {index} does not exist.");
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing to remove in this server.");
        }

        [Command("clearplaylist", RunMode = RunMode.Async)]
        public async Task ClearPlaylist()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                if (lavaPlayer.PlayerState == PlayerState.Playing || lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];
                    guildMusicState.ClearPlaylist();
                    await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, "Cleared playlist.");
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing to clear in this server.");
        }

        [Command("lyrics", RunMode = RunMode.Async)]
        public async Task Lyrics()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                if (lavaPlayer.PlayerState == PlayerState.Playing || lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                    string lyrics = await guildMusicState.CurrentTrack.FetchLyricsFromGeniusAsync();
                    if (string.IsNullOrWhiteSpace(lyrics))
                    {
                        await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"No lyrics found for {guildMusicState.CurrentTrack.Title}");
                        return;
                    }

                    string[] lyricLines = lyrics.Split('\n');
                    UnicodeEncoding encoding = new UnicodeEncoding();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StreamWriter sw = new StreamWriter(ms, encoding);
                        try
                        {
                            for (int i = 0; i < lyricLines.Length; i++) await sw.WriteLineAsync(lyricLines[i]);
                            await sw.FlushAsync();
                            ms.Seek(0, SeekOrigin.Begin);
                            await Context.Channel.SendFileAsync(ms, "Lyrics.txt");
                        }
                        finally
                        {
                            await sw.DisposeAsync();
                        }
                        ms.Close();
                    }
                    return;
                }
            }
            await Context.Channel.SendErrorEmbedAsync($"{user.Mention} | Nothing playing to get lyrics in this server.");
        }
        #endregion

        #region Button Commands
        [Command("pausetoggle", RunMode = RunMode.Async)]
        public async Task PauseToggle()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                await guildMusicState.DeleteMessage("pause");
                await guildMusicState.DeleteMessage("resume");
                await guildMusicState.DeleteMessage("stop");

                if (lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    await lavaPlayer.ResumeAsync();
                    return;
                }

                if (lavaPlayer.PlayerState == PlayerState.Playing) await lavaPlayer.PauseAsync();
            }
        }

        [Command("volumedown", RunMode = RunMode.Async)]
        public async Task VolumeDown()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                guildMusicState.Volume = Math.Clamp(guildMusicState.Volume - 10, 0, 200);
                await lavaPlayer.UpdateVolumeAsync((ushort)guildMusicState.Volume);
                await guildMusicState.DeleteMessage("volume");
            }
        }

        [Command("volumeup", RunMode = RunMode.Async)]
        public async Task VolumeUp()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                guildMusicState.Volume = Math.Clamp(guildMusicState.Volume + 10, 0, 200);
                await lavaPlayer.UpdateVolumeAsync((ushort)guildMusicState.Volume);
                await guildMusicState.DeleteMessage("volume");
            }
        }

        [Command("silentskip", true, RunMode = RunMode.Async)]
        public async Task SilentSkip()
        {
            IUser user = Context.InteractionState == null ? Context.User : Context.InteractionState.CommandInteraction.User;

            if (Context.Module.LavaNodeService.TryGetPlayer(Context.Guild, out LavaPlayer lavaPlayer))
            {
                if (lavaPlayer.PlayerState == PlayerState.Playing || lavaPlayer.PlayerState == PlayerState.Paused)
                {
                    GuildMusicState guildMusicState = Context.Module.GuildMusicStates[Context.Guild.Id];

                    guildMusicState.PendingIgnoreStop = true;
                    await lavaPlayer.StopAsync();
                    await guildMusicState.DeleteMessage("skip");
                }
            }
        }
        #endregion
    }
}
