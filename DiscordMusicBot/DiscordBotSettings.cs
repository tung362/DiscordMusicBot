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
using DiscordMusicBot.Modules.Music;

namespace DiscordMusicBot
{
    /// <summary>
    /// Represents a DiscordBotClient settings instance
    /// </summary>
    public class DiscordBotSettings
    {
        private string _CommandPrefix { get; set; }
        private UserStatus _ClientStatus { get; set; }
        private ActivityType _ClientActivity { get; set; }
        public ServiceCollection ClientModules { get; private set; }
        public Dictionary<string, string> Paths { get; private set; }

        /*Properties*/
        public string CommandPrefix
        { 
            get { return _CommandPrefix; }
            set
            {
                _CommandPrefix = value;
                OnCommandPrefixChange?.Invoke(_CommandPrefix);
            }
        }
        public UserStatus ClientStatus
        {
            get { return _ClientStatus; }
            set
            {
                _ClientStatus = value;
                OnStatusChange?.Invoke(_ClientStatus);
            }
        }
        public ActivityType ClientActivity
        {
            get{ return _ClientActivity; }
            set
            {
                _ClientActivity = value;
                OnActivityChange?.Invoke(_ClientActivity);
            }
        }

        /*Events*/
        public event Action<string> OnCommandPrefixChange;
        public event Action<UserStatus> OnStatusChange;
        public event Action<ActivityType> OnActivityChange;

        public DiscordBotSettings()
        {
            ClientModules = new ServiceCollection();
            Paths = new Dictionary<string, string>();
            ClientModules.AddSingleton<DiscordSocketClient>();
            ClientModules.AddSingleton<CommandService>();
        }
    }
}
