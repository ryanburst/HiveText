# Hive Text

The bees have something to say! This mod adds the ability to pull a random line of text from a list and use that instead
of the built in text. The bees aren't just happy, "The bees are overjoyed!"

## Description

The Hive Text mod adds a `.yml` configuration file that provides several options (list key) to list lines of text that can be pulled
randomly. Each list key corresponds to an in-game interaction with the `Beehive` prefab, with an additional `GenericText` list key 
that can be used in the absense of another list key.

A few options have been provided for you in the default `.yml` config file that gets generated when you boot the game. Because this mod
uses `ServerSync`, you can change anything in the `.yml` file and it will be updated even when the game has already been loaded.

The Hive Text `.cfg` file contains mod options for enabling or disabling the use of random text for each interaction. If an interaction
is enabled, but there is no corresponding list key in the `.yml` file, it will pull from the `GenericText` list key instead. By doing 
this, you can provide text specific to a certain interaction **or** just random text.

`Version checks with itself. If installed on the server, it will kick clients who do not have it installed.`

`This mod uses ServerSync, if installed on the server and all clients, it will sync all configs to client`

`This mod uses a file watcher. If the configuration file is not changed with BepInEx Configuration Manager, but changed in the file directly on the server, upon file save, it will sync the changes to all clients.`

## Installation Instructions

### Manual Installation

`Note: (Manual installation is likely how you have to do this on a server, make sure BepInEx is installed on the server correctly)`

1. **Download the latest release of BepInEx.**
2. **Extract the contents of the zip file to your game's root folder.**
3. **Download the latest release of HiveText from Thunderstore.io.**
4. **Extract the contents of the zip file to the `BepInEx/plugins` folder.**
5. **Launch the game.**

### Installation through r2modman or Thunderstore Mod Manager

1. **Install [r2modman](https://valheim.thunderstore.io/package/ebkr/r2modman/)
   or [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager).**

   > For r2modman, you can also install it through the Thunderstore site.
   ![](https://i.imgur.com/s4X4rEs.png "r2modman Download")

   > For Thunderstore Mod Manager, you can also install it through the Overwolf app store
   ![](https://i.imgur.com/HQLZFp4.png "Thunderstore Mod Manager Download")
2. **Open the Mod Manager and search for "HiveText" under the Online
   tab.
3. **Click the Download button to install the mod.**
4. **Launch the game** with the "Start modded" option.

## Configuration
### HiveText.cfg
#### 1 - General
|Name|Server Sync|Description|Default|
|-----|----------|--------|-----|
|Lock Configuration|Yes|If on, the configuration is locked and can be changed by server admins only.|On|

#### 2 - Enable Random Text
|Name|Server Sync|Description|Default|
|-----|----------|--------|-----|
|SleepText|Yes|Enable pulling random lines of text to replace the sleeping text.|Enabled|
|BiomeText|Yes|Enable pulling random lines of text to replace the biome text.|Enabled|
|FreespaceText|Yes|Enable pulling random lines of text to replace the freespace text.|Enabled|
|HappyText|Yes|Enable pulling random lines of text to replace the happy text.|Enabled|
|ExtractedText|Yes|Enable pulling random lines of text to when honey is extracted from the hive.|Enabled|
|AttackedText|Yes|nable pulling random lines of text when a beehive takes damage from a player.|Enabled|

### HiveText.yml
|List Key|Description|
|---|---|
|SleepText|Replaces "The bees are sleeping" text that shows at night or during a storm. Stop disturbing the bees.|
|BiomeText|Replaces "The bees don't like this biome." text that shows if you build the hive in a biome they don't like. Picky bees.|
|SpaceText|Replaces "The bees need more open space." text when there is too much overhead coverage. Let the sun shine!|
|HappyText|Replaces "The bees are happy." text when checking a beehive and none of the other conditions are met. Bee Ross says "Happy bees! Happy little bees!"|
|ExtractedText|Doesn't replace any existing text, but shows when you successfully pull honey from a hive. You need it more than them, right?|
|AttackedText|Doesn't replace any existing text, but shows when you damage a beehive. Do you know how many times I've accidentally punched a hive making this mod? The bees do. And they don't forget.|
|GenericText (default)|The fallback list. If any of the above options are enabled in the `.cfg` file, but don't exist in the `.yml` file, it will pull from here.|

## Author Information
### WildGrue
`STEAM:` https://steamcommunity.com/id/wildgrue/
`GITHUB:` https://github.com/ryanburst/

### Credits
This mod was created using [Azumatt's](https://github.com/AzumattDev) Piece Manager [Mod Template](https://github.com/AzumattDev/PieceManagerModTemplate).
While I didn't end up using the actual `PieceManager` class, the structure of the template and most of my knowledge of how to mod comes from him and his
[tutorials](https://www.youtube.com/watch?v=TIKZruMg0k0&list=PLyVQ1HvkGPdrJygqisb6kSC5CfsUvHXJF). The tutorials are *slightly* out of date, but after poking
around a bit and with some general coding knowledge you can get everything running. 
