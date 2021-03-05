using System;
using System.IO;
using System.Reflection;
using Harmony;
using Base.Build;
using System.Linq;
using PhoenixPoint.Home.View.ViewModules;
//using AssortedAdjustments.Patches;

namespace AssortedAdjustments
{
    public static class AssortedAdjustments
    {
        internal static string LogPath;
        internal static string ModDirectory;
        internal static Settings Settings;
        internal static string[] ValidPresets = new string[] { "vanilla", "hardcore", "mad" };
        internal static HarmonyInstance Harmony;

        internal static string ModName = "AssortedAdjustments";
        internal static Version ModVersion;



        // Modnix Entrypoints
        public static void SplashMod(Func<string, object, object> api)
        {
            Harmony = HarmonyInstance.Create("de.mad.AssortedAdjustments");

            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogPath = Path.Combine(ModDirectory, "AssortedAdjustments.log");
            Settings = api("config", null) as Settings ?? new Settings();
            
            if ((!String.IsNullOrEmpty(Settings.DebugDevKey) && Settings.DebugDevKey == "mad") || (!String.IsNullOrEmpty(Settings.BalancePresetId) && Settings.BalancePresetId == "mad"))
            {
                Settings.DebugLevel = 3;
            }
            Logger.Initialize(LogPath, Settings.DebugLevel, ModDirectory, nameof(AssortedAdjustments));

            object ModInfo = api("mod_info", null);
            ModVersion = (Version)ModInfo.GetType().GetField("Version").GetValue(ModInfo);



            if (!String.IsNullOrEmpty(Settings.BalancePresetId) && ValidPresets.Any(p => Settings.BalancePresetId.Contains(p)))
            {
                PresetHelpers.HandlePresets(ref Settings, api);
            }
            if (Settings.EnableLimitedWar)
            {
                LimitedWar.Config.MergeSettings(Settings);
            }



            if (Settings.SkipIntroLogos)
            {
                HarmonyHelpers.Patch(Harmony, typeof(PhoenixPoint.Common.Game.PhoenixGame), "RunGameLevel", typeof(Patches.SkipIntro), "Prefix_PhoenixGame_RunGameLevel");
            }
            if (Settings.SkipIntroMovie)
            {
                HarmonyHelpers.Patch(Harmony, typeof(PhoenixPoint.Home.View.ViewStates.UIStateHomeScreenCutscene), "EnterState", typeof(Patches.SkipIntro), null, "Postfix_UIStateHomeScreenCutscene_EnterState");
            }
            if (Settings.SkipLandingSequences)
            {
                HarmonyHelpers.Patch(Harmony, typeof(PhoenixPoint.Tactical.View.ViewStates.UIStateTacticalCutscene), "EnterState", typeof(Patches.SkipIntro), null, "Postfix_UIStateTacticalCutscene_EnterState");
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
                Patches.EconomyAdjustments.Apply();
            }

            if (Settings.EnableFacilityAdjustments)
            {
                Patches.FacilityAdjustments.Apply();
            }

            if (Settings.EnableSoldierAdjustments)
            {
                Patches.SoldierAdjustments.Apply();
            }

            if (Settings.EnableVehicleAdjustments)
            {
                Patches.VehicleAdjustments.Apply();
            }

            if (Settings.EnableMissionAdjustments)
            {
                Patches.MissionAdjustments.Apply();
            }

            if (Settings.EnableCustomRecruitGeneration)
            {
                Patches.CustomRecruitGeneration.Apply();
            }

            if (Settings.EnableDifficultyOverrides)
            {
                Patches.DifficultyOverrides.Apply();
            }

            if (Settings.EnableAbilityAdjustments)
            {
                Patches.AbilityAdjustments.Apply();
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
