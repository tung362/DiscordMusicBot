using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Modules.Core
{
    public class AdminCommands : ModuleBase<CoreCommandContext>
    {
        #region Commands
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("updateglobalslashcommands")]
        public async Task UpdateGlobalSlashCommands()
        {
            try
            {
                List<SlashCommandProperties> slashCommands = new List<SlashCommandProperties>();
                foreach (KeyValuePair<string, SlashCommandBuilder> kv in Context.Module.DiscordBot.GlobalSlashCommands) slashCommands.Add(kv.Value.Build());
                await Context.Module.DiscordBot.Client.BulkOverwriteGlobalApplicationCommandsAsync(slashCommands.ToArray());
                await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Updated global slash commands.");
            }
            catch (HttpException exception)
            {
                await LogUtils.LogAsync($"Client: {exception.Reason}", Context.Module.DiscordBot.LogPath);
                await Context.Channel.SendErrorEmbedAsync($"{exception.Reason}");
            }
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("updateguildslashcommands")]
        public async Task UpdateGuildSlashCommands()
        {
            try
            {
                List<SlashCommandProperties> slashCommands = new List<SlashCommandProperties>();
                foreach (KeyValuePair<string, SlashCommandBuilder> kv in Context.Module.DiscordBot.GuildSlashCommands) slashCommands.Add(kv.Value.Build());
                await Context.Guild.BulkOverwriteApplicationCommandsAsync(slashCommands.ToArray());
                await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Updated guild slash commands.");
            }
            catch (HttpException exception)
            {
                await LogUtils.LogAsync($"Client: {exception.Reason}", Context.Module.DiscordBot.LogPath);
                await Context.Channel.SendErrorEmbedAsync($"{exception.Reason}");
            }
        }

        [RequireOwner]
        [Command("leaveallguilds")]
        public async Task LeaveAllGuilds()
        {
            foreach (SocketGuild guild in Context.Module.DiscordBot.Client.Guilds) await guild.LeaveAsync();
        }
        #endregion
    }
}
