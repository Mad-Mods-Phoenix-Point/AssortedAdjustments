using System;
using System.IO;
using System.Reflection;
using Harmony;
using AssortedAdjustments.Patches;
using Base.Build;
using System.Linq;
using PhoenixPoint.Home.View.ViewModules;

namespace AssortedAdjustments
{
    public static class AssortedAdjustments
    {
        internal static string LogPath;
        internal static string ModDirectory;
        internal static Settings Settings;
        internal static HarmonyInstance Harmony;

        // BEN: DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        internal static int DebugLevel = 1;

        internal static string ModName = "AssortedAdjustments";
        internal static Version ModVersion;



        // Modnix Entrypoints
        public static void SplashMod(Func<string, object, object> api)
        {
            Harmony = HarmonyInstance.Create("de.mad.AssortedAdjustments");

            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogPath = Path.Combine(ModDirectory, "AssortedAdjustments.log");
            Settings = api("config", null) as Settings ?? new Settings();

            object ModInfo = api("mod_info", null);
            ModVersion = (Version)ModInfo.GetType().GetField("Version").GetValue(ModInfo);



            if (!String.IsNullOrEmpty(Settings.BalancePreset))
            {
                if (Settings.BalancePreset == "vanilla")
                {
                    Settings.DisableRocksAtBases = false;
                    Settings.EnableCustomRecruitGeneration = false;
                    Settings.EnableDifficultyOverrides = false;
                    Settings.EnableExperienceToSkillpointConversion = false;
                    Settings.EnableMissionAdjustments = false;
                    Settings.EnablePlentifulItemDrops = false;
                    Settings.EnableFacilityAdjustments = false;
                    Settings.EnableSoldierAdjustments = false;
                    Settings.EnableVehicleAdjustments = false;
                    Settings.EnableEconomyAdjustments = false;
                }
                else if (Settings.BalancePreset == "hardcore")
                {
                    Settings.DisableRocksAtBases = false;

                    Settings.EnableCustomRecruitGeneration = true;
                    Settings.RecruitGenerationCount = 1;
                    Settings.IgnoreRngFactorsForHavenRecruitGeneration = false;
                    Settings.RecruitIntervalCheckDays = 9f;
                    Settings.RecruitGenerationHasArmor = false;
                    Settings.RecruitGenerationHasWeapons = false;
                    Settings.RecruitGenerationHasConsumableItems = false;
                    Settings.RecruitGenerationHasInventoryItems = false;

                    Settings.EnableDifficultyOverrides = true;
                    Settings.DifficultyOverrideStartingSupplies = 150f;
                    Settings.DifficultyOverrideStartingMaterials = 300f;
                    Settings.DifficultyOverrideStartingTech = 50f;
                    Settings.DifficultyOverrideSoldierSkillPointsPerMission = 4;
                    Settings.DifficultyOverrideExpConvertedToSkillpoints = 0.005f;
                    Settings.DifficultyOverrideMinPopulationThreshold = 25;
                    Settings.DifficultyOverrideStarvationDeathsPart = 0.02f;
                    Settings.DifficultyOverrideStarvationMistDeathsPart = 0.04f;
                    Settings.DifficultyOverrideStarvationDeathsFlat = 10;
                    Settings.DifficultyOverrideStarvationMistDeathsFlat = 20;

                    Settings.EnableExperienceToSkillpointConversion = false;

                    Settings.EnableMissionAdjustments = false;

                    Settings.EnablePlentifulItemDrops = false;

                    Settings.EnableFacilityAdjustments = true;
                    Settings.MedicalBayBaseHeal = 3f;
                    Settings.LivingQuartersBaseStaminaHeal = 1f;
                    Settings.VehicleBayAircraftHealAmount = 1;
                    Settings.VehicleBayVehicleHealAmount = 15;
                    Settings.MutationLabMutogHealAmount = 15;
                    Settings.TrainingFacilityBaseExperienceAmount = 1;
                    Settings.TrainingFacilityBaseSkillPointsAmount = 1;
                    Settings.FabricationPlantGenerateProductionAmount = 3f;
                    Settings.ResearchLabGenerateResearchAmount = 3f;
                    Settings.FoodProductionGenerateSuppliesAmount = 0.25f;
                    Settings.BionicsLabGenerateResearchAmount = 3f;
                    Settings.MutationLabGenerateMutagenAmount = 0.25f;
                    Settings.FabricationPlantGenerateMaterialsAmount = 0f;
                    Settings.ResearchLabGenerateTechAmount = 0f;

                    Settings.EnableSoldierAdjustments = true;
                    Settings.MaxAugmentations = 1;
                    Settings.PersonalAbilitiesCount = 3;
                    Settings.MaxStrength = 25;
                    Settings.MaxWill = 20;
                    Settings.MaxSpeed = 20;

                    Settings.EnableVehicleAdjustments = false;

                    Settings.EnableEconomyAdjustments = true;
                    Settings.ResourceMultiplier = 1.1f;
                    Settings.ScrapMultiplier = 0.25f;
                    Settings.CostMultiplier = 1.1f;

                    Settings.DisableAmbushes = false;
                }
            }



            // Apply my own custom settings that differ from "popular demands"
            if (!String.IsNullOrEmpty(Settings.DebugDevKey) && Settings.DebugDevKey == "mad")
            {
                Settings.AgendaTrackerHideStatusBar = true;
                Settings.PersistentClassFilterInitDisabled = true;
                Settings.BuffTutorialSquad = true;
                Settings.DifficultyOverrideExpConvertedToSkillpoints = 0.03f;
                Settings.MaxPlayerUnitsAdd = 1;
                Settings.MedicalBayBaseHeal = 4; // Vanilla default
                Settings.LivingQuartersBaseStaminaHeal = 2; // Vanilla default
                Settings.VehicleBayAircraftHealAmount = 2; // Vanilla default
                Settings.VehicleBayVehicleHealAmount = 20; // Vanilla default
                Settings.MutationLabMutogHealAmount = 20; // Vanilla default
                Settings.AircraftBlimpSpeed = 250; // Vanilla default
                Settings.AircraftThunderbirdSpeed = 380; // Vanilla default
                Settings.AircraftManticoreSpeed = 500; // Vanilla default
                Settings.AircraftHeliosSpeed = 650; // Vanilla default
                Settings.AircraftBlimpSpace = 9;
                Settings.CostMultiplier = 0.75f;
                Settings.MaxWill = 25;
                Settings.EnableAbilityAdjustments = true;
                Settings.PauseOnExplorationSet = true;
                Settings.DebugLevel = 3;
            }

            if (Settings.Debug && Settings.DebugLevel > 0)
            {
                DebugLevel = Settings.DebugLevel > 3 ? 3 : Settings.DebugLevel;
            }
            Logger.Initialize(LogPath, DebugLevel, ModDirectory, nameof(AssortedAdjustments));



            if (Settings.SkipIntroLogos)
            {
                HarmonyHelpers.Patch(Harmony, typeof(PhoenixPoint.Common.Game.PhoenixGame), "RunGameLevel", typeof(SkipIntro), "Prefix_PhoenixGame_RunGameLevel");
            }
            if (Settings.SkipIntroMovie)
            {
                HarmonyHelpers.Patch(Harmony, typeof(PhoenixPoint.Home.View.ViewStates.UIStateHomeScreenCutscene), "EnterState", typeof(SkipIntro), null, "Postfix_UIStateHomeScreenCutscene_EnterState");
            }
            if (Settings.SkipLandingSequences)
            {
                HarmonyHelpers.Patch(Harmony, typeof(PhoenixPoint.Tactical.View.ViewStates.UIStateTacticalCutscene), "EnterState", typeof(SkipIntro), null, "Postfix_UIStateTacticalCutscene_EnterState");
            }



            Logger.Always($"Modnix Mad.AssortedAdjustments.SplashMod initialised.");
            //Logger.Always($"Settings: {Settings}");
            


            try
            {
                Settings.ToMarkdownFile(Path.Combine(ModDirectory, "settings-reference.md"));
                Settings.ToHtmlFile(Path.Combine(ModDirectory, "settings-reference.htm"));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void MainMod(Func<string, object, object> api)
        {
            DataHelpers.Print();
            Harmony.PatchAll();
            ApplyAll();

            Logger.Always($"Modnix Mad.AssortedAdjustments.MainMod initialised.");
            Logger.Always($"GameVersion: {RuntimeBuildInfo.BuildVersion}");
            Logger.Always($"ModVersion: {ModVersion}");
        }



        public static void ApplyAll()
        {
            if(Settings.EnableEconomyAdjustments)
            {
                EconomyAdjustments.Apply();
            }

            if (Settings.EnableFacilityAdjustments)
            {
                FacilityAdjustments.Apply();
            }

            if (Settings.EnableSoldierAdjustments)
            {
                SoldierAdjustments.Apply();
            }

            if (Settings.EnableVehicleAdjustments)
            {
                VehicleAdjustments.Apply();
            }

            if (Settings.EnableMissionAdjustments)
            {
                MissionAdjustments.Apply();
            }

            if (Settings.EnableCustomRecruitGeneration)
            {
                CustomRecruitGeneration.Apply();
            }

            if (Settings.EnableDifficultyOverrides)
            {
                DifficultyOverrides.Apply();
            }

            if (Settings.EnableAbilityAdjustments)
            {
                AbilityAdjustments.Apply();
            }
        }



        [HarmonyPatch(typeof(UIModuleBuildRevision), "SetRevisionNumber")]
        public static class UIModuleBuildRevision_SetRevisionNumber_Patch
        {
            public static void Postfix(UIModuleBuildRevision __instance)
            {
                try
                {
                    __instance.BuildRevisionNumber.text = $"{RuntimeBuildInfo.UserVersion} w/{ModName} {ModVersion}";
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
