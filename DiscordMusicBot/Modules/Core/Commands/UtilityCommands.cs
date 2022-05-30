using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Modules.Core
{
    /// <summary>
    /// Handles help commands
    /// </summary>
    public class UtilityCommands : ModuleBase<CoreCommandContext>
    {
        #region Commands
        [Command("help")]
        public async Task Help()
        {
            EmbedBuilder eb = new EmbedBuilder
            {
                Color = ColorUtils.MainColor,
                Title = $"🍌 {Context.Module.DiscordBot.Client.CurrentUser.Username}'s Help List",
                Description = $"Welcome to {Context.Module.DiscordBot.Client.CurrentUser.Username}'s help page",
            };
            for (int i = 0; i < Context.Module.DiscordBot.CommandCategoriesCount; i++)
            {
                DiscordCommandCategory commandCategory = Context.Module.DiscordBot.GetCommandCategory(i);
                string discription = "";
                for (int y = 0; y < commandCategory.CommandInfos.Count; y++)
                {
                    DiscordCommandInfo commandInfo = commandCategory.CommandInfos[y];
                    discription += $"`{commandInfo.CommandName}`";
                    if (y != commandCategory.CommandInfos.Count - 1) discription += ", ";
                }
                eb.AddField(EmbedUtils.CreateEmbedField(commandCategory.CategoryName, discription, false));
            }
            eb.AddField(EmbedUtils.CreateEmbedField("☕ Want to buy me a coffee?", $"[Donate](https://www.paypal.com/donate/?business=UPQ757L48DWRA&currency_code=USD)", false));
            eb.WithFooter($"Use {Context.Module.DiscordBot.Settings.CommandPrefix}help <command> to get more information about a command.");
            eb.WithThumbnailUrl(Context.Module.DiscordBot.Client.CurrentUser.GetAvatarUrl());
            await Context.Channel.SendMessageAsync("", false, eb.Build());
        }

        [Command("help")]
        public async Task Help(string commandName)
        {
            if (Context.Module.DiscordBot.TryGetCommandInfo(commandName, out DiscordCommandInfo commandInfo)) await Context.Channel.SendCommandInfo(commandInfo, Context.Module.DiscordBot.Settings.CommandPrefix);
        }
        #endregion
    }
}
