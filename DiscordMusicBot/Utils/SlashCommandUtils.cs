using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordMusicBot
{
    public static class SlashCommandUtils
    {
        public static SlashCommandBuilder CreateSlashCommand(string name, string description)
        {
            SlashCommandBuilder scb = new SlashCommandBuilder
            {
                Name = name,
                Description = description
            };
            return scb;
        }

        public static SlashCommandOptionBuilder CreateSlashCommandOption(string name, string description, ApplicationCommandOptionType optionType, bool isRequired = true)
        {
            SlashCommandOptionBuilder scob = new SlashCommandOptionBuilder
            {
                Name = name,
                Description = description,
                Type = optionType,
                IsRequired = isRequired
            };
            return scob;
        }
    }
}
