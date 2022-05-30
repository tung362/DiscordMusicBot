using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot
{
    public static class EmbedUtils
    {
        public static async Task<IUserMessage> SendErrorEmbedAsync(this IMessageChannel channel, string text, MessageComponent messageComponent = null)
        {
            EmbedBuilder eb = new EmbedBuilder
            {
                Color = Color.Red,
                Title = "⚠️ Error",
                Description = text

            };
            return await channel.SendMessageAsync("", false, eb.Build(), components: messageComponent);
        }

        public static async Task<IUserMessage> SendNormalEmbedAsync(this IMessageChannel channel, Color color, string text, string title = null, string footer = null, string thumbnailUrl = null, MessageComponent messageComponent = null)
        {
            EmbedBuilder eb = new EmbedBuilder
            {
                Color = color,
                Description = text,
            };
            if (!string.IsNullOrEmpty(title)) eb.WithTitle(title);
            if (!string.IsNullOrEmpty(footer)) eb.WithFooter(footer);
            if (!string.IsNullOrEmpty(thumbnailUrl)) eb.WithThumbnailUrl(thumbnailUrl);
            return await channel.SendMessageAsync("", false, eb.Build(), components: messageComponent);
        }

        public static async Task<IUserMessage> SendCommandInfo(this IMessageChannel channel, DiscordCommandInfo commandInfo, string commandPrefix, MessageComponent messageComponent = null)
        {
            EmbedBuilder eb = new EmbedBuilder
            {
                Color = ColorUtils.MainColor,
                Title = commandInfo.CommandName,
                Description = commandInfo.Description
            };
            string usagesText = "";
            for (int i = 0; i < commandInfo.Usages.Count; i++)
            {
                usagesText += $"`{commandPrefix}{commandInfo.Usages[i]}`";
                if (i != commandInfo.Usages.Count - 1) usagesText += "\n";
            }
            eb.AddField(CreateEmbedField("Usages", usagesText, false));

            string examplesText = "";
            for (int i = 0; i < commandInfo.Examples.Count; i++)
            {
                examplesText += $"`{commandPrefix}{commandInfo.Examples[i]}`";
                if (i != commandInfo.Examples.Count - 1) examplesText += "\n";
            }
            eb.AddField(CreateEmbedField("Examples", examplesText, false));
            return await channel.SendMessageAsync("", false, eb.Build(), components: messageComponent);
        }

        public static EmbedFieldBuilder CreateEmbedField(string title, string discription, bool inline)
        {
            EmbedFieldBuilder efb = new EmbedFieldBuilder();
            efb.WithName(title);
            efb.WithValue(discription);
            efb.WithIsInline(inline);
            return efb;
        }
    }
}
