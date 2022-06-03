# DiscordMusicBot
Discord music bot using Lavalink with various BDO REST API integrations.  
No pesky premiums included!  

![alt text](https://i.imgur.com/MyxkHR2.png)  

### Dependencies:  
- .NET 6.0  
- [Newtonsoft.Json 13.0.1](https://www.nuget.org/packages/Newtonsoft.Json/13.0.1/)  
- [Microsoft.Extensions.DependencyInjection 6.0.0](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/6.0.0/)  
- [Discord.Net 3.5.0](https://www.nuget.org/packages/Discord.Net/3.5.0/)  
- [Victoria 5.2.8](https://www.nuget.org/packages/Victoria/5.2.8/)  
- Java 16  

### Features:  
- Music module  
    - Plays Youtube, Soundcloud, and direct links  
    - Ability to loop the current sound track or playlist
    - Ability to skip sound tracks
    - Ability to seek to sound tracks in the playlist
    - Ability to pause or resume playing sound tracks
    - Ability to change the volume of the music player
    - Ability to grab the lyrics from specific sound tracks
    - Ability to edit the playlist
- Boss timer module (Still under development)  
- Market module (Still under development)  
- Slash commands
- Expandable framework for adding new modules  

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
9. Type command `!updateglobalslashcommands` to register the global slash commands (Takes about an hour to show up after registering)
10. Type command `!updateguildslashcommands` on your desired discord server to register guild slash commands (Can be used immediately after registering)

### Configuration:  
1. Navigate to `\Configs`  

### Screenshots:  
![alt text](https://i.imgur.com/WIgcHz9.png)  
![alt text](https://i.imgur.com/M2ePitt.png)  
![alt text](https://i.imgur.com/IE15Ei4.png)  
![alt text](https://i.imgur.com/DX71won.png)  
![alt text](https://i.imgur.com/SqSxATc.png)  
