# DiscordMusicBot
Discord music bot with BDO REST API integration

### Dependencies:  
- .NET 6.0  
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/13.0.1/)  
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/6.0.0/)  
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/3.5.0/)  
- [Victoria 5.2.8](https://www.nuget.org/packages/Victoria/5.2.8/)  
- Java 16  

### How to use:  
1. Navigate to `\Configs`  
2. Open `BotConfig.txt` with any text editor  
3. Change the `Token` entry to your bot's token  
4. If you are using the music module follow these steps otherwise skip to the next step  
    1. Setup a static IP with the desired IP  
    2. Setup a port forward with the desired port  
    3. Navigate to `\Lavalink`  
    4. Open `application.yml` with any text editor  
    5. Change the `port` entry to the desired port in step 2
    6. Change the `address` entry to the desired IP in step 1
    7. Run `Run.bat` (runs the Lavalink server)
6. Navigate back to the root folder  
7. run `DiscordMusicBot.exe` (runs the discord bot)
8. Everything should work fine without errors if all steps were followed

### Configuration:  
1. Navigate to `\Configs`  

