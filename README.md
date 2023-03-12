# WindowsGSM.MemoriesOfMars
🧩 WindowsGSM plugin that provides Memories of Mars Dedicated server support!


# WindowsGSM Installation: 
1. Download  WindowsGSM https://windowsgsm.com/ 
2. Create a Folder at a Location you wan't all Server to be Installed and Run.
4. Drag WindowsGSM.Exe into previoulsy created folder and execute it.

# Plugin Installation:
1. Download [latest](https://github.com/ohmcodes/WindowsGSM.MemoriesOfMars/releases/latest) release
2. Extract then Move **MemoriesOfMars.cs** folder to **plugins** folder
3. OR Press on the Puzzle Icon in the left bottom side and install this plugin by navigating to it and select the Zip File.
4. Click **[RELOAD PLUGINS]** button or restart WindowsGSM
5. Navigate "Servers" and Click "Install Game Server" and find "Memories of Mars Dedicated Server [MemoriesOfMars.cs]

### Official Documentation
🗃️ https://505games.com/wp-content/uploads/Handbook-Dedicated-Servers-Memories-of-Mars-V1.5.pdf

### The Game
🕹️ https://store.steampowered.com/app/644290/MEMORIES_OF_MARS/

### Dedicated server info
🖥️ https://steamdb.info/app/897590/info/

#### Change configuration manually
- Open WGSM Go to "Browse" Tab and Click "Server Files"
- Find "DedicatedServerConfig.cfg" for server settings
- Find "CustomRuleSet.cfg" for game settings

#### NOTE
- This plugin automatically overrides command-line parameters when you start the server so no need to set extra param for this game server
- All overrides config located on WindowsGSM.cfg
- No need to change maxplayers on DedicatedServerConfig.cfg
- No need to add Port and SteamQueryPort on DedicatedServerConfig.cfg
```
-MULTIHOME= {automatically gets the local ip of the current machine. this can be change on Server IP Address}
-PublicIP= {automatically gets the public ip of your ISP}
-port= {can be change on Server Port}
-beaconport= {can be change on Server Query Port}
-maxplayers= {can be change on Server Maxplayers}
```


#### More Configurations
- If you set "DailyRestartUTCHour" make sure to turn on "Auto Start" so whenever the server shuts down it will restart.

# License
This project is licensed under the MIT License - see the <a href="https://github.com/ohmcodes/WindowsGSM.MemoriesOfMars/blob/main/LICENSE">LICENSE.md</a> file for details
