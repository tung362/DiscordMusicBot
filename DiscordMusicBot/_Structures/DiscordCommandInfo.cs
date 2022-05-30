using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Discord;

namespace DiscordMusicBot
{
    public class DiscordCommandInfo
    {
        public readonly string CommandName;
        public readonly string Description;
        public IReadOnlyList<string> Usages;
        public IReadOnlyList<string> Examples;

        public DiscordCommandInfo(string commandName, string description, List<string> usages, List<string> examples)
        {
            CommandName = commandName;
            Description = description;
            Usages = ImmutableList.CreateRange(usages);
            Examples = ImmutableList.CreateRange(examples);
        }

        public DiscordCommandInfo(string path)
        {
            if (ConfigUtils.LoadConfig(path, out Dictionary<string, string> config))
            {
                List<string> usages = new List<string>();
                List<string> examples = new List<string>();

                //Apply
                CommandName = config["CommandName"];
                Description = config["Description"];
                int i = 0;
                while (true)
                {
                    bool hasMatches = false;
                    if (config.ContainsKey($"Usage{i}"))
                    {
                        usages.Add(config[$"Usage{i}"]);
                        hasMatches = true;
                    }

                    if (config.ContainsKey($"Example{i}"))
                    {
                        examples.Add(config[$"Example{i}"]);
                        hasMatches = true;
                    }

                    if (!hasMatches)
                    {
                        Usages = ImmutableList.CreateRange(usages);
                        Examples = ImmutableList.CreateRange(examples);
                        return;
                    }
                    i++;
                }
            }
        }
    }
}
