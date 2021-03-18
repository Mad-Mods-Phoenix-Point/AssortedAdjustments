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
4. Check your final folder structure, it should contain:
    - /AssortedAdjustments/AssortedAdjustments.dll
    - /AssortedAdjustments/README.md
    - /AssortedAdjustments/changelog.txt
5. With the mod installed and selected in Modnix, have a look in the lower left section 
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

### Balance Presets
Upon popular demand you can set a balance preset which overrides the mod's general defaults if set to recognized values:  
* "vanilla": This will disable all difficulty-related changes and only keep general enhancements
* "hardcore": Sets many variables to be even harder than legendary

#### Guide
Since version 1.10.0.2 i use Modnix's API to save the preset-related settings back to its .conf-file if a valid preset is detected.  
With that change you can  
* Set a preset at "BalancePresetId" and <b>after you loaded up the game once</b>...
* Change the config to your liking even if your changes partially affect the preset
* Play with a custom config that is <b>based upon</b> a preset

To make it easier for you to trust this system there's the settings-field "BalancePresetState" which displays a keyword describing the preset's state:
* "SET" -> All values in your config match the relevant settings of the preset, you're all set :-)
* "CUSTOMIZED" -> You have changed a value in your config which overrides the desired value of the preset. At this point you're on your own.
* "RESET" -> This will never get set automatically, <b>you</b> can set it it to <b>force all related values</b> back to the preset's default.

#### Long story short
All this is only relevant if you have entered a valid preset at "BalancePresetId".  
If you're happy with your current config just ignore this option.  
If you think this mod makes things too easy for you, set "vanilla", play legendary and enjoy the QOL/UI enhancements.  
If you wanna suffer, set "hardcore" and try to make it through.  


## Features (may not be complete, mod default)

### UX & UI
* The global agenda tracker (or however you call it) now shows vehicle-related (travel and exploration times) AND excavation-related items.
    * Added the functionality to focus on related site/activity when an item is clicked.
    * Facilities under repair will also show.
    * Once an excavation is done, a new secondary mission objective (to seize control of the site) is added.
    * Optionally hide the ugly big buttons that show excavation times in vanilla.
    * The context menu will show expected times behind the action too (Borrowed from Sheepy).
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
* Objectives for haven defenses will now show attacking faction

### Bugfixes
* Vanillas broken ability generation sometimes caused soldiers to have less than the expected number of personal skills
    * This is fixed and supports up to 7 personal abilities for every soldier.
    * UI fixes and enhancements included.
* Vanillas post-mission replenishment module potentially shows items of dismissed/dead characters
    * Happens for example when you dismiss soldiers while their loadout is incomplete
    * This is fixed.
* Removing items from the manufacturing queue now works properly all the time

### Gameplay
* Maxed out soldiers now gain additional SP for their wasted XP (Rebuilds Sheepy's "Spill Exp to Skill").
    * The converted skillpoints can be assigned to the soldier's pool and/or the global pool (default: global pool).
* Return Fire has a (configurable, default: 180) maximum reaction angle and a (configurable, default: 2) shot limit per turn (Thx pantolomin for the inspiration).
    * Return Fire will be canceled if the shooter steps out of full cover
    * Emphasized visuals of return fire indicator icons
* Squad limit in missions can be configured (Vanilla: 8, Default: 10 (+2)).
* All missions can be set to always collect any dropped items (Default: off).
* Adjust item drop rate (lower destruction chance) and allow weapons to drop (Thx RealityMachina for the inspiration). All configurable.
    * By default weapon drops are health dependent, so completely undamaged weapons always drop
* Prepared some soldier attributes to be changed via settings.
    * You can change Strength, Willpower, Speed (no changes by default)
    * Personal abilities limit raised to 5 (configurable). Note that this in <b>not</b> applied retroactively but only affects freshly generated characters.
    * Stamina is raised to 50 by default, soldiers are tired/exhausted at 15/5.
* Aircraft speeds and space slightly raised. You can configure that too.
* Available ammunition for ground vehicles raised.
* Reduced manufacturing times and resource costs for all items. Raised their scrap values.
* Disable ambushes but retained them if inside the mist (Thx Sheepy).
* Disable the "Nothing found" event.
* Disable the annoying "Right Click Move" in tactical missions. God.
* Disable the rock tiles in bases. 
* Keep the game paused when ordering vehicles to travel
* Pause the game when a squad is rested (Thx Sheepy).
* Pause the game when new recruits have arrived at phoenix bases.
* Allow full mutations AND full augmentations for the soldiers along with UI fixes for the related screens.
* Recruits at phoenix bases now come with armor and equipment by default.
* The evacuation prompt in tactical missions will now only show if the whole squad is ready to exit the mission.
* You can now scrap aircraft from the roster list.
    * If you move all units out of the aircraft the formerly "Empty"-Slot will now trigger the option to scrap the vehicle.
    * You can't scrap your last aircraft because the game bugs out totally in geoscape without any vehicle to select.
* Starting resources and general skillpoint gains are adjustable now, by default they resemble the game's easy setting.
* You can set the size of groundehicles and mutogs (space occupied in aircraft, default: 2)
* Exposed some diffculty settings regarding population census (the "doom meter").
    * You can reduce (or completely disable) the death rate of havens caused by starvation
    * By default the values are equivalent to vanilla's settings
* Fresh recruits at havens will spawn every 3 days (Vanilla: 7)
* Limit faction wars to a configurable intensity (Thanks Sheepy for his excellent blueprint). You can:
    * Limit haven attacks from factions to a global cap
    * Limit attacks per faction
    * Stop one faction overrunning another
    * Boost haven defenses to make destructions less likely
    * Convert potential haven destruction to a less severe zone destruction
    * See settings reference for details
* Certain items will be available for production after their (custom) research requirements are met
    * Phoenix "Elite" Gear (the "golden" items with adjusted stats)
	* Independent Ammo
    * Independent Weapons (Default: off)
    * Independent Armor (Default: off)
	* Living Weapons (Default: off)
    * Living Armor (Default: off)

#### Facilities
* Medical bays and vehicle bays have their base healing/repair rate doubled.
* Food production base output slightly raised.
* Fabrication plants generate 1(one) Material per day.
* Research labs generate 1(one) Tech per day.
* Prepared several other values to be changed via settings.
* All values configurable.

#### Abilities
* You can enable some minor adjustments to a few tactical abilities (Default: off).
    * "RetrieveTurret" will cost only 1AP (Vanilla: 2AP)
    * "BigBooms" will cost only 4WP (Vanilla: 5WP)
    * "Cautious" will only lose 5% damage (Vanilla: 10%)
    * "Reckless" will only lose 5% accuracy (Vanilla: 10%)
    * "Strongman" will only lose 10 perception (Vanilla: 15)
    * "Frenzy" will only boost speed by 33% (Vanilla: 50%)
    * "RemoveFacehugger" will cost only 1AP (Vanilla: 2AP)

#### Items
* You can enable some minor adjustments to a few tactical items (Default: off).
    * "Subjector" (Subjugator) is slightly more effective
    * "HeavyRocketLauncher" will have some weapon spread
    * "ShotgunRifle" (Mercy SG) will be slightly more accurate
    * "LaserArrayPack" (Destiny III) does less damage and has the "ExplosiveWeapon"-Tag removed

### QOL
* Intro logos and introductary movie will get skipped (Sheepy's "Skip Intro" can do even more, if you want to use that, please disable these via settings).
* Saving of soldiers loadouts is optimized to also respect the order of items

## Settings (again)
Everything is configurable. I tried to be very clear in naming the settings so everything should be more or less self-explanatory.  
<strike>In the future i hope i can add a detailed table with the Setting, its default value and an extensive description.</strike>  
Every time the mod is loaded it will generate a markdown-file AND a html-file in the mod-folder. There you can see all settings explained along with thir default values.  
Where appropiate i added a hint to the vanilla default value.  
Furthermore, all values marked <b>bold</b> differ from the <b>mods default</b> and thus reflect all changes you (or a preset) made in the configuration of this mod.
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
Tested with GOG Year One Edition (1.10).  
Some features are derivates of existent mods which are adapted, sometimes fixed to work with the YOE version of the game.  
If the original mods work for you and/or you just like them better, please disable the corresponding feature in the settings of this mod.

## Thanks
* Sheepy
* RealityMachina
* pantolomin
* pardeike
* Snapshot Games
* Sheepy (again, because once is not enough)