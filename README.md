# Assorted Adjustments

[Phoenix Point Mod][Modnix] Various little tweaks to adjust the game to your liking.

## Installation and Configuration
[tl;dr:] Download .zip, open Modnix 2.5.5+, click "Add Mod", select .zip, open "Config-Tab", configure, save, run game.

### Installation
1. Make sure you have Modnix installed, Version 2.5.5 and up are recommended
    - https://github.com/Sheep-y/Modnix/releases/tag/v2.5.5
    - https://www.nexusmods.com/phoenixpoint/mods/43
2. Download this mod
    - https://github.com/Mad-Mods-Phoenix-Point/AssortedAdjustments/releases
    - https://www.nexusmods.com/phoenixpoint/mods/53
3. Follow this guide
    - https://github.com/Sheep-y/Modnix/wiki/Add-Mod
4. With the mod installed and selected in Modnix, have a look in the lower left section 
    - There should be a "Config"-Tab, open it
    - Feel overwhelmed by the wall of text, breathe and set a few configuration options that are self-explanatory
    - Sneak peak at https://github.com/Mad-Mods-Phoenix-Point/AssortedAdjustments/blob/main/Source/AssortedAdjustments/Settings.cs
    - Click "Save" and launch the game

### Settings
Some options are probably hard to understand for new players or just badly named (sry).  
In that case, <b>after</b> you launched the game with the mod activated for the first time, you can have a look at the <b>reference</b>.  
- Open `%UserProfile%\Documents\My Games\Phoenix Point\Mods\AssortedAdjustments\`
- Open `settings-reference.htm` or `settings-reference.md`, whatever format you like more
- Read about the settings that you don't understand, understand them, and change them in <b>Modnix's Config</b>
    - The file is <b>just a source of information</b>, not a configuration file!
- If some values in the reference are marked <b>bold</b> it means that your currently set value is different from the mod's default
    - Just to help you keep track of yourself.

## Features (may not be complete)

### UX
* The global agenda tracker (or however you call it) now shows vehicle-related (travel and exploration times) AND excavation-related items.
    * Added the functionality to focus on related site/activity when an item is clicked.
    * Facilities under repair will also show.
    * Once an excavation is done, a new secondary mission objective (to seize control of the site) is added.
    * Optionally hide the ugly big buttons that show excavation times in vanilla.
    * The context menu will show expected times behind the action too (Borrowed from Sheepy).

### UI
* Smart base selection. Ever wondered why the bases menu didn't show the base your aircraft just arrived at as selected by default?
    * Before opening the bases gui the most relevant (that is the closest to your screen's center) base is prepared to be shown as default.
    * Also works when panning around freely and opening the menu while above a base
* Research and Production points are shown in Info-Bar and the Manufacture/Research-Queues. To make a bit more sense of the required times.
* The tooltip at recruitment zones in havens now show the personal abilities of the recruit (Adapted from Sheepy's "Recruit Info").
* Extended information for bases.
    * Popup on activated Phoenix Bases will show healing and repair capabilities.
    * Added popup in base overview to show the accumulated healing/resources output, soldiers in treatment and vehicle details.
    * Added popup in recruitment screen for the selected base with extended info too.
* Added trade information to haven popups along with the recruits class (if any).

### Bugfixes
* Vanillas broken ability generation sometimes caused soldiers to have less than the expected number of personal skills
    * This is fixed and supports up to 7 personal abilities for every soldier.
    * UI fixes and enhancements included.

### Gameplay
* Maxed out soldiers now gain additional SP for their wasted XP (Rebuilds Sheepy's "Spill Exp to Skill").
    * The converted skillpoints can be assigned to the soldier's pool or the global pool (or both).
* Return Fire has a (configurable) maximum reaction angle and a (configurable) shot limit per turn (Thx pantolomin for the inspiration).
* Squad limit in missions can be configured (Vanilla: 8, Default: 10).
* All missions can be set to always collect any dropped items (Default: off).
* Adjust item drop rate (lower destruction chance) and allow weapons to drop (Thx RealityMachina for the inspiration). All configurable.
* Prepared some soldier attributes to be changed via settings (Default: off).
* Aircraft speeds and space slightly raised. You can configure that too.
* Reduced manufacturing times and resource costs for all items. Raised their scrap values.
* Disable ambushes but retained them if inside the mist (Thx Sheepy).
* Disable the "Nothing found" event.
* Disable the annoying "Right Click Move" in tactical missions. God.
* Disable the rock tiles in bases. 
* Keep the game paused when ordering vehicles to travel and when a squad is rested (Thx Sheepy).
* Pause the game when new recruits have arrived at phoenic bases.
* Allow full mutations AND full augmentations for the soldiers along with UI fixes for the related screens.
* Recruitment of soldiers with more than 3 personal abilities at phoenix bases now works!
* Recruits at Phoenic bases now come with armor and equipment by default.
* The evacuation prompt in tactical missions will now only show if the whole squad is ready to exit the mission.
* You can now scrap aircraft from the roster list.
    * If you move all units out of the aircraft the formerly "Empty"-Slot will now trigger the option to scrap the vehicle.
    * You can't scrap your last aircraft because the game bugs out totally in geoscape without any vehicle to select.

#### Facilities
* Medical bays and vehicle bays have their base healing/repair rate doubled.
* Food production base output slightly raised.
* Fabrication plants generate 1(one) Material per day.
* Research labs generate 1(one) Tech per day.
* Prepared several other values to be changed via settings.
* All values configurable.

### QOL
* Intro logos and introductary movie will get skipped (Sheepy's "Skip Intro" can do even more, if you want to use that, please disable these via settings).

## Settings (again)
Everything is configurable. I tried to be very clear in naming the settings so everything should be more or less self-explanatory.  
<strike>In the future i hope i can add a detailed table with the Setting, its default value and an extensive description.</strike>  
Every time the mod is loaded it will generate a markdown-file AND a html-file in the mod-folder. There you can see all settings explained along with thir default values.  
Where appropiate i added a hint to the vanilla default value.  
Furthermore, all values marked <b>bold</b> differ from the mods default and thus reflect all changes you made in the configuration of this mod.
Feel free to comment if sth. is unclear or add a pull request at: 
- https://github.com/Mad-Mods-Phoenix-Point/AssortedAdjustments.

### Reference

Please refer to the settings-reference in `%UserProfile%\Documents\My Games\Phoenix Point\Mods\AssortedAdjustments\` for further information about the settings.  
Please note that this reference will be generated at runtime, so you'll need to at least once load up the game with the mod activated.
Please note that it is A REFERENCE only, changing settings still needs to be done in Modnix.

### Obsolete with Modnix 2.5.5 and up
If you have problems with finding/applying the settings please see:  
- https://github.com/Sheep-y/Modnix/wiki/User-Guide
    - Especially: https://github.com/Sheep-y/Modnix/wiki/Add-Mod
    - Preferred way of adding: Use the "Add Mod" Button in Modnix and select the .zip 
- https://stackoverflow.com/questions/28339116/not-allowed-to-load-assembly-from-network-location

## Notes
Requires Modnix 2.5.5 or higher.  
Tested with GOG Year One Edition (1.9).  
Some features are derivates of existent mods which are adapted, sometimes fixed to work with the YOE version of the game.  
If the original mods work for you and/or you just like them better, please disable the corresponding feature in the settings of this mod.

## Thanks
* Sheepy
* RealityMachina
* pantolomin
* pardeike
* Snapshot Games
* Sheepy (again, because once is not enough)