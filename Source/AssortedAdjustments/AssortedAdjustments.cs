using System;
using System.IO;
using System.Reflection;
using Harmony;
using AssortedAdjustments.Patches;

namespace AssortedAdjustments
{
    public static class AssortedAdjustments
    {
        internal static string LogPath;
        internal static string ModDirectory;
        internal static Settings Settings;
        internal static HarmonyInstance Harmony;

        // BEN: DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        internal static int DebugLevel = 0;



        // Modnix Entrypoints
        public static void SplashMod(Func<string, object, object> api)
        {
            Harmony = HarmonyInstance.Create("de.mad.AssortedAdjustments");

            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogPath = Path.Combine(ModDirectory, "AssortedAdjustments.log");
            Settings = api("config", null) as Settings ?? new Settings();



            // Apply my own custom settings that differ from "popular demands"
            if (!String.IsNullOrEmpty(Settings.DebugDevKey) && Settings.DebugDevKey == "mad")
            {
                Settings.AgendaTrackerHideStatusBar = true;

                Settings.PersistentClassFilterInitDisabled = true;

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
            Logger.Always($"Settings: {Settings}");


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
            //ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //LogPath = Path.Combine(ModDirectory, "AssortedAdjustments.log");
            //Logger.Initialize(LogPath, DebugLevel, ModDirectory, nameof(AssortedAdjustments));
            //Settings = api("config", null) as Settings ?? new Settings();

            DataHelpers.Print();
            Harmony.PatchAll();
            ApplyAll();

            Logger.Always($"Modnix Mad.AssortedAdjustments.MainMod initialised.");
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
        }
    }
}
