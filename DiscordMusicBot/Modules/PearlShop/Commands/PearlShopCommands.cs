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

namespace DiscordMusicBot.Modules.PearlShop
{
    public class PearlShopCommands : ModuleBase<PearlShopCommandContext>
    {
        #region Commands
        [Command("test")]
        public async Task Test()
        {
            await Context.Channel.SendNormalEmbedAsync(ColorUtils.MainColor, $"Test");
        }
        #endregion
    }
}
