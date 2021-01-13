using System;
using System.Reflection;

namespace AssortedAdjustments
{
    internal class Settings
    {
        [Annotation("Disables direct right click movement.", "True", true, "General")]
        public bool DisableRightClickMove = true;
        [Annotation("Disables the rock tiles in phoenix bases completely.", "True")]
        public bool DisableRocksAtBases = true;
        [Annotation("Will only show the confirmation popup when moving a unit to the evacuation zone if the whole squad is ready to evacuate. You can still evacuate single units by using the ability bar.", "True")]
        public bool EnableSmartEvacuation = true;
        [Annotation("Will preselect the closest phoenix base to the screen's center when entering the bases menu at the bottom.", "True")]
        public bool EnableSmartBaseSelection = true;


        [Annotation("General switch to enable the related subfeatures.", "True", true, "UI Enhancements")]
        public bool EnableUIEnhancements = true;
        [Annotation("Shows current production and research points behind the facility count and adds some related information to the manufacturing and research screens.", "True")]
        public bool ShowDetailedResearchAndProduction = true;
        [Annotation("Shows personal abilities and augmentations (if any) of recruits in havens.", "True")]
        public bool ShowRecruitInfoInsideZoneTooltip = true;
        [Annotation("Adds vehicle-related entries (travel and exploration times) to the agenda tracker above the time controller.", "True")]
        public bool ShowTravelAgenda = true;
        [Annotation("Adds current healing rates to the bases tooltip in geoscape. Adds tooltips to the left-hand side menu in the bases screen and the bases info in recruitment screen.", "True")]
        public bool ShowExtendedBaseInfo = true;
        [Annotation("Adds trade information and recruit class/level to the haven popups.", "True")]
        public bool ShowExtendedHavenInfo = true;
        [Annotation("The class filter in the manufacturing screen now remembers its state when leaving/re-entering or switching from manufacture to scrap.", "True")]
        public bool PersistentClassFilter = true;
        [Annotation("Will start the manufacturing screen with all class filters deselected.", "False")]
        public bool PersistentClassFilterInitDisabled = false;
        [Annotation("Hides addons of havens/bases on the geoscape (ie. mist repeller).", "False")]
        public bool HideSiteAddons = false;



        [Annotation("General switch to enable the related subfeatures.", "True", true, "Recruit Generation")]
        public bool EnableCustomRecruitGeneration = true;
        [Annotation("Fixed amount of recruits that get generated for phoenix faction. Note that more than 3 looks like shit because of the broken UI. Note that the UI cannot handle more than 4 at all.", "3")]
        public int RecruitGenerationCount = 3;
        [Annotation("New recruits may have armor.", "True")]
        public bool RecruitGenerationHasArmor = true;
        [Annotation("New recruits may have weapons.", "True")]
        public bool RecruitGenerationHasWeapons = true;
        [Annotation("New recruits may have consumables.", "True")]
        public bool RecruitGenerationHasConsumableItems = true;
        [Annotation("New recruits may have items.", "True")]
        public bool RecruitGenerationHasInventoryItems = true;
        [Annotation("New recruits may have augmentations already installed.", "False")]
        public bool RecruitGenerationCanHaveAugmentations = false;



        [Annotation("Fully leveled soldiers will convert some experience to skill points. Base rate is dependent on difficulty setting, somewhere between 1 and 3 percent.", "True", true, "Progression")]
        public bool EnableExperienceToSkillpointConversion = true;
        internal float XPtoSPConversionRate = 0.01f; // Default is dependent on difficulty setting, this is just a fallback if the setting is unretrievable.
        [Annotation("Will multiply the converted skill points by its value.", "2")]
        public float XPtoSPConversionMultiplier = 2f;



        [Annotation("General switch to enable the related subfeatures", "True", true, "Return Fire")]
        public bool EnableReturnFireAdjustments = true;
        [Annotation("Maximum angle in which return fire is possible at all. Vanilla didn't check at all, returned fire for 360 degrees.", "225")]
        public int ReturnFireAngle = 225;
        [Annotation("Limit the ability to return fire to X times per round, vanilla default is unlimited.", "2")]
        public int ReturnFireLimit = 2;



        [Annotation("General switch to enable the related subfeatures", "True", true, "Mission Adjustments")]
        public bool EnableMissionAdjustments = true;
        [Annotation("Adds to the maximum squad size for tactical missions. With a current vanilla default of 8, a value of 2 means you can bring up to 10 soldiers.", "2")]
        public int MaxPlayerUnitsAdd = 2;
        [Annotation("Some missions require your soldiers to actually carry loot home. This would disable that requirement. Makes scavenging missions somewhat pointless.", "False")]
        public bool AlwaysRecoverAllItemsFromTacticalMissions = false;



        [Annotation("General switch to enable the related subfeatures", "True", true, "")]
        public bool EnablePlentifulItemDrops = true;
        [Annotation("Base chance for items to be destroyed when dropped by a dying enemy. Vanilla defaults to 80 percent for most items.", "10")]
        public int ItemDestructionChance = 10;
        [Annotation("Allows for weapons to drop too.", "True")]
        public bool AllowWeaponDrops = true;
        [Annotation("If weapon drops are allowed, this is the chance for them to be destroyed nonetheless.", "30")]
        public int FlatWeaponDestructionChance = 30;
        [Annotation("If weapon drops are allowed, use a health based percentage to determine if the dropped weapon will be destroyed. Weapons with full health will thus have a 100% chance to drop.", "True")]
        public bool HealthBasedWeaponDestruction = true;
        [Annotation("Allows for armor to drop too.", "True")]
        public bool AllowArmorDrops = true;



        [Annotation("Skips logos when loading up the game.", "True", true, "Cinematics")]
        public bool SkipIntroLogos = true;
        [Annotation("Skips intro movie too.", "True")]
        public bool SkipIntroMovie = true;
        [Annotation("Skips landing sequences before tactical missions too.", "True")]
        public bool SkipLandingSequences = true;


        [Annotation("General switch to enable the related subfeatures", "True", true, "Facility modifications")]
        public bool EnableFacilityAdjustments = true;
        // Healing
        [Annotation("Healing rate for medical bays, vanilla default is 4", "8")]
        public float MedicalBayBaseHeal = 8f;
        [Annotation("Stamina regeneration rate for living quarters, vanilla default is 2", "4")]
        public float LivingQuartersBaseStaminaHeal = 4f;
        [Annotation("Healing rate for aircraft at vehicle bays, vanilla default is 2. NOTE that this seems to be obsolete by now, vanilla also uses healing rate of vehicles for aircraft.", "4")]
        public int VehicleBayAircraftHealAmount = 4;
        [Annotation("Healing rate for vehicles at vehicle bays, vanilla default is 20", "40")]
        public int VehicleBayVehicleHealAmount = 40;
        [Annotation("Healing rate for mutogs at mutation labs, vanilla default is 20", "40")]
        public int MutationLabMutogHealAmount = 40;
        //Training
        [Annotation("Experience gain rate for soldiers at training facilities, vanilla default is 2", "2")]
        public int TrainingFacilityBaseExperienceAmount = 2;
        // Resource Generators
        [Annotation("Production points generated at fabrication plants, vanilla default is 4", "4")]
        public float FabricationPlantGenerateProductionAmount = 4f;
        [Annotation("Research points generated at research labs, vanilla default is 4", "4")]
        public float ResearchLabGenerateResearchAmount = 4f;
        [Annotation("Supplies (Food) generated at food production facilities, vanilla default is 0.33 (translates to 8 food/day).", "0.5")]
        public float FoodProductionGenerateSuppliesAmount = 0.5f;
        [Annotation("Research points generated at bionic labs, vanilla default is 4", "4")]
        public float BionicsLabGenerateResearchAmount = 4f;
        [Annotation("Mutagen points generated at mutation labs, vanilla default is 0.25 (translates to 6 mutagen/day).", "0.25")]
        public float MutationLabGenerateMutagenAmount = 0.25f;
        // Add Tech & Materials to Facilities
        [Annotation("Fabrication plant generate this amount of materials per day.", "1")]
        public float FabricationPlantGenerateMaterialsAmount = 1f;
        [Annotation("Research labs generate this amount of tech per day.", "1")]
        public float ResearchLabGenerateTechAmount = 1f;
        internal float GenerateResourcesBaseDivisor = 24f;



        [Annotation("General switch to enable the related subfeatures", "True", true, "Soldiers")]
        public bool EnableSoldierAdjustments = true;
        [Annotation("Augmentation limit, vanilla default is 2", "3")]
        public int MaxAugmentations = 3;
        [Annotation("Personal skill limit, vanilla default is 3", "5")]
        public int PersonalAbilitiesCount = 5;
        [Annotation("Maximum strength, vanilla default is 30", "30")]
        public int MaxStrength = 30;
        [Annotation("Maximum willpower, vanilla default is 20", "20")]
        public int MaxWill = 20;
        [Annotation("Maximum speed, vanilla default is 20", "20")]
        public int MaxSpeed = 20;



        [Annotation("General switch to enable the related subfeatures.", "True", true, "Vehicles")]
        public bool EnableVehicleAdjustments = true;
        [Annotation("Maximum speed for the Tiamat, vanilla default is 250", "300")]
        public float AircraftBlimpSpeed = 300f;
        [Annotation("Maximum speed for the Thunderbird, vanilla default is 380", "400")]
        public float AircraftThunderbirdSpeed = 400f;
        [Annotation("Maximum speed for the Manticore, vanilla default is 500", "550")]
        public float AircraftManticoreSpeed = 550f;
        [Annotation("Maximum speed for the Helios, vanilla default is 650", "700")]
        public float AircraftHeliosSpeed = 700f;
        [Annotation("Maximum soldier capacity for the Tiamat, vanilla default is 8", "10")]
        public int AircraftBlimpSpace = 10;
        [Annotation("Maximum soldier capacity for the Thunderbird, vanilla default is 7", "8")]
        public int AircraftThunderbirdSpace = 8;
        [Annotation("Maximum soldier capacity for the Manticore, vanilla default is 6", "7")]
        public int AircraftManticoreSpace = 7;
        [Annotation("Maximum soldier capacity for the Helios, vanilla default is 5", "6")]
        public int AircraftHeliosSpace = 6;



        [Annotation("General switch to enable the related subfeatures.", "True", true, "Economy")]
        public bool EnableEconomyAdjustments = true;
        [Annotation("General multiplier for manufacturing costs.", "0.75")]
        public float ResourceMultiplier = 0.75f;
        [Annotation("General multiplier for scrapping costs, vanilla default is 0.5", "0.5")]
        public float ScrapMultiplier = 0.5f;
        [Annotation("General multiplier for manufacturing times.", "0.5")]
        public float CostMultiplier = 0.5f;



        [Annotation("Disables ambushes when exploring sites.", "True", true, "Events")]
        public bool DisableAmbushes = true;
        [Annotation("When sites are inside the mist ambushes are still a possibility.", "True")]
        internal bool RetainAmbushesInsideMist = true;
        [Annotation("Suppresses the 'Nothing found' event when exploring sites.", "True")]
        public bool DisableNothingFound = true;



        [Annotation("Keeps the game paused/pauses the game when setting a new target for an aircraft.", "True", true, "Pause and Center")]
        public bool PauseOnDestinationSet = true;
        [Annotation("Keeps the game paused/pauses the game when exploring a new site.", "False")]
        public bool PauseOnExplorationSet = false;
        [Annotation("Pauses the game when new recruits have arrived at phoenix bases.", "True")]
        public bool PauseOnRecruitsGenerated = true;
        [Annotation("Pauses the game when a squad is fully rested.", "True")]
        public bool PauseOnHealed = true;
        internal bool CenterOnHealed = true;
        [Annotation("Centers view on the haven that was just discovered.", "True")]
        public bool CenterOnHavenRevealed = true;



        [Annotation("Enables some logging if errors occur", "True", true, "Do not touch")]
        public bool Debug = true;
        internal int DebugLevel = 1;
        [Annotation("Magical setting that allows the developer to override some settings for personal use.", "")]
        public string DebugDevKey = "";



        public override string ToString()
        {
            string result = "";
            Type t = this.GetType();
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo fi in fields)
            {
                result += "\n";
                result += fi.Name;
                result += ": ";
                result += fi.GetValue(this).ToString();
            }
            return result;
        }



        public void ToHtmlFile(string destination)
        {
            string result = "<!doctype html><html lang=en><head><meta charset=utf-8><title>Assorted Adjustments: Settings</title><style>html {font-family: sans-serif;} body {padding:2em;} h1 {padding-left: 10px;} th {font-size:1.4em;}</style></head><body>\n";
            result += "<h1>SETTINGS</h1>\n";

            result += "<table cellpadding=0 cellspacing=10>\n";
            result += $"<tr><th align=left>Name</th><th align=left>Value</th><th align=left>Description</th><th align=right>Default</th></tr>\n";

            Type t = this.GetType();
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo fi in fields)
            {
                Annotation annotation = Attribute.IsDefined(fi, typeof(Annotation)) ? (Annotation)Attribute.GetCustomAttribute(fi, typeof(Annotation)) : null;

                string settingName = fi.Name;
                string setValue = fi.GetValue(this).ToString();
                string settingDesc = annotation?.Description;
                string defaultValue = annotation?.DefaultValue;

                if (annotation?.DefaultValue != null && setValue != defaultValue)
                {
                    setValue = $"<b>{setValue}</b>";
                }

                if (annotation != null && annotation.StartSection)
                {
                    result += $"<tr><td colspan=4><br><b>{annotation.SectionLabel}</b></td></tr>\n";
                }

                result += "<tr><td>";
                result += $" {settingName} ";
                result += "</td><td>";
                result += $" {setValue} ";
                result += "</td><td>";
                result += $" {settingDesc} ";
                result += "</td><td align=right>";
                result += $" <i>{defaultValue}</i> ";
                result += "</td></tr>\n";
            }
            result += "</table></body></html>";

            System.IO.File.WriteAllText(destination, result);
        }



        public void ToMarkdownFile(string destination)
        {
            string result = "";
            result += "# SETTINGS";
            result += "\n\n";

            result += $"|Name|Value|Description|Default|\n";
            result += $"|:---|:----|:----------|:-----:|\n";

            Type t = this.GetType();
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo fi in fields)
            {
                Annotation annotation = Attribute.IsDefined(fi, typeof(Annotation)) ? (Annotation)Attribute.GetCustomAttribute(fi, typeof(Annotation)) : null;

                string settingName = fi.Name;
                string setValue = fi.GetValue(this).ToString();
                string settingDesc = annotation?.Description;
                string defaultValue = annotation?.DefaultValue;

                if (annotation?.DefaultValue != null && setValue != defaultValue)
                {
                    setValue = $"<b>{setValue}</b>";
                }

                if (annotation != null && annotation.StartSection)
                {
                    result += $"| . | . | . | . |\n";
                    result += $"| . | . | . | . |\n";
                    result += $"| . | . | . | . |\n";
                    result += $"| <b>{annotation.SectionLabel}</b> | | | |\n";
                }

                result += "|";
                result += $" {settingName} ";
                result += "|";
                result += $" {setValue} ";
                result += "|";
                result += $" {settingDesc} ";
                result += "|";
                result += $" <i>{defaultValue}</i> ";
                result += "|\n";
            }

            System.IO.File.WriteAllText(destination, result);
        }
    }
}
