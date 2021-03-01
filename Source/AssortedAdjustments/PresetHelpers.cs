using System;
using System.Collections.Generic;

namespace AssortedAdjustments
{
    internal static class PresetHelpers
    {
        private const string stateNotSet = "INIT";
        private const string stateSet = "SET";
        private const string stateCustomized = "CUSTOMIZED";

        public static void HandlePresets(ref Settings settings, Func<string, object, object> api)
        {
            // Vanilla
            Settings PresetVanilla = api("config", null) as Settings;
            PresetVanilla.DisableRocksAtBases = false;
            PresetVanilla.EnableCustomRecruitGeneration = false;
            PresetVanilla.EnableDifficultyOverrides = false;
            PresetVanilla.EnableExperienceToSkillpointConversion = false;
            PresetVanilla.EnableMissionAdjustments = false;
            PresetVanilla.EnablePlentifulItemDrops = false;
            PresetVanilla.EnableFacilityAdjustments = false;
            PresetVanilla.EnableSoldierAdjustments = false;
            PresetVanilla.EnableVehicleAdjustments = false;
            PresetVanilla.EnableEconomyAdjustments = false;

            // Hardcore
            Settings PresetHardcore = api("config", null) as Settings;
            PresetHardcore.DisableRocksAtBases = false;
            PresetHardcore.EnableCustomRecruitGeneration = true;
            PresetHardcore.RecruitGenerationCount = 1;
            PresetHardcore.IgnoreRngFactorsForHavenRecruitGeneration = false;
            PresetHardcore.RecruitIntervalCheckDays = 9f;
            PresetHardcore.RecruitGenerationHasArmor = false;
            PresetHardcore.RecruitGenerationHasWeapons = false;
            PresetHardcore.RecruitGenerationHasConsumableItems = false;
            PresetHardcore.RecruitGenerationHasInventoryItems = false;
            PresetHardcore.EnableDifficultyOverrides = true;
            PresetHardcore.DifficultyOverrideStartingSupplies = 150f;
            PresetHardcore.DifficultyOverrideStartingMaterials = 300f;
            PresetHardcore.DifficultyOverrideStartingTech = 50f;
            PresetHardcore.DifficultyOverrideSoldierSkillPointsPerMission = 4;
            PresetHardcore.DifficultyOverrideExpConvertedToSkillpoints = 0.005f;
            PresetHardcore.DifficultyOverrideMinPopulationThreshold = 25;
            PresetHardcore.DifficultyOverrideStarvationDeathsPart = 0.02f;
            PresetHardcore.DifficultyOverrideStarvationMistDeathsPart = 0.04f;
            PresetHardcore.DifficultyOverrideStarvationDeathsFlat = 10;
            PresetHardcore.DifficultyOverrideStarvationMistDeathsFlat = 20;
            PresetHardcore.EnableExperienceToSkillpointConversion = false;
            PresetHardcore.EnableMissionAdjustments = false;
            PresetHardcore.EnablePlentifulItemDrops = false;
            PresetHardcore.EnableFacilityAdjustments = true;
            PresetHardcore.MedicalBayBaseHeal = 3f;
            PresetHardcore.LivingQuartersBaseStaminaHeal = 1f;
            PresetHardcore.VehicleBayAircraftHealAmount = 1;
            PresetHardcore.VehicleBayVehicleHealAmount = 15;
            PresetHardcore.MutationLabMutogHealAmount = 15;
            PresetHardcore.TrainingFacilityBaseExperienceAmount = 1;
            PresetHardcore.TrainingFacilityBaseSkillPointsAmount = 1;
            PresetHardcore.FabricationPlantGenerateProductionAmount = 3f;
            PresetHardcore.ResearchLabGenerateResearchAmount = 3f;
            PresetHardcore.FoodProductionGenerateSuppliesAmount = 0.25f;
            PresetHardcore.BionicsLabGenerateResearchAmount = 3f;
            PresetHardcore.MutationLabGenerateMutagenAmount = 0.25f;
            PresetHardcore.FabricationPlantGenerateMaterialsAmount = 0f;
            PresetHardcore.ResearchLabGenerateTechAmount = 0f;
            PresetHardcore.EnableSoldierAdjustments = true;
            PresetHardcore.MaxAugmentations = 1;
            PresetHardcore.PersonalAbilitiesCount = 3;
            PresetHardcore.MaxStrength = 25;
            PresetHardcore.MaxWill = 20;
            PresetHardcore.MaxSpeed = 20;
            PresetHardcore.EnableVehicleAdjustments = false;
            PresetHardcore.EnableEconomyAdjustments = true;
            PresetHardcore.ResourceMultiplier = 1.1f;
            PresetHardcore.ScrapMultiplier = 0.25f;
            PresetHardcore.CostMultiplier = 1.1f;
            PresetHardcore.DisableAmbushes = false;

            // Mad
            //Settings PresetMad = api("config", null) as Settings;
            Settings PresetMad = new Settings();
            PresetMad.BalancePresetId = "mad";

            PresetMad.AgendaTrackerHideStatusBar = true;
            PresetMad.PersistentClassFilterInitDisabled = true;
            PresetMad.EnhanceSpecialRecruits = true;
            PresetMad.DifficultyOverrideExpConvertedToSkillpoints = 0.03f;
            PresetMad.DifficultyOverrideMinPopulationThreshold = 10;
            PresetMad.MaxPlayerUnitsAdd = 1;
            PresetMad.MedicalBayBaseHeal = 4; // Vanilla default
            PresetMad.LivingQuartersBaseStaminaHeal = 2; // Vanilla default
            PresetMad.VehicleBayAircraftHealAmount = 2; // Vanilla default
            PresetMad.VehicleBayVehicleHealAmount = 20; // Vanilla default
            PresetMad.MutationLabMutogHealAmount = 20; // Vanilla default
            PresetMad.AircraftBlimpSpeed = 250; // Vanilla default
            PresetMad.AircraftThunderbirdSpeed = 380; // Vanilla default
            PresetMad.AircraftManticoreSpeed = 500; // Vanilla default
            PresetMad.AircraftHeliosSpeed = 650; // Vanilla default
            PresetMad.AircraftBlimpSpace = 9;
            PresetMad.CostMultiplier = 0.75f;
            PresetMad.MaxWill = 25;
            PresetMad.EnableAbilityAdjustments = true;
            PresetMad.PauseOnExplorationSet = true;

            Dictionary<string, Settings> Presets = new Dictionary<string, Settings>();
            Presets.Add("vanilla", PresetVanilla);
            Presets.Add("hardcore", PresetHardcore);
            Presets.Add("mad", PresetMad);

            foreach (KeyValuePair<string, Settings> preset in Presets)
            {
                if (settings.BalancePresetId.Contains(preset.Key))
                {
                    // Check for preset change
                    if (settings.PresetStateHash != settings.BalancePresetId.GetHashCode())
                    {
                        string msg = $"Preset set/changed! Resetting state!";
                        api("log warn", msg);
                        Logger.Always($"[Utilities_HandlePresets] {msg}");

                        settings.BalancePresetState = stateNotSet;
                    }

                    // Current settings match preset
                    if (settings.Equals(preset.Value))
                    {
                        if (settings.BalancePresetState == stateCustomized)
                        {
                            string msg = $"Settings match! BalancePresetId: {preset.Key}, State: {settings.BalancePresetState}. Marking preset as {stateSet} and keeping config as set in Modnix.";
                            api("log info", msg);
                            Logger.Always($"[Utilities_HandlePresets] {msg}");

                            settings.BalancePresetState = stateSet;
                            api("config save", settings);
                        }
                        else if (settings.BalancePresetState == stateSet)
                        {
                            string msg = $"Settings match! BalancePresetId: {preset.Key}, State: {settings.BalancePresetState}. Keeping config as set in Modnix.";
                            api("log info", msg);
                            Logger.Always($"[Utilities_HandlePresets] {msg}");
                        }
                        else
                        {
                            string msg = $"Settings match! BalancePresetId: {preset.Key}, State: {settings.BalancePresetState}. Marking preset as {stateSet} and keeping config as set in Modnix.";
                            api("log info", msg);
                            Logger.Always($"[Utilities_HandlePresets] {msg}");

                            settings.BalancePresetState = stateSet;
                            settings.PresetStateHash = preset.Key.GetHashCode();
                            api("config save", settings);
                        }
                    }
                    // Current settings differ from preset
                    else
                    {
                        if (settings.BalancePresetState == stateSet)
                        {
                            string msg = $"Settings mismatch! BalancePresetId: {preset.Key}, State: {settings.BalancePresetState}. Marking preset as {stateCustomized} and keeping config as set in Modnix.";
                            api("log info", msg);
                            Logger.Always($"[Utilities_HandlePresets] {msg}");

                            settings.BalancePresetState = stateCustomized;
                            api("config save", settings);
                        }
                        else if (settings.BalancePresetState == stateCustomized)
                        {
                            string msg = $"Settings mismatch! BalancePresetId: {preset.Key}, State: {settings.BalancePresetState}. Keeping config as set in Modnix.";
                            api("log info", msg);
                            Logger.Always($"[Utilities_HandlePresets] {msg}");
                        }
                        else
                        {
                            string msg = $"Settings mismatch! BalancePresetId: {preset.Key}, State: {settings.BalancePresetState}. Overriding relevant fields, marking preset as {stateSet} and saving config to Modnix.";
                            api("log info", msg);
                            Logger.Always($"[Utilities_HandlePresets] {msg}");

                            preset.Value.BalancePresetState = stateSet;
                            preset.Value.PresetStateHash = preset.Key.GetHashCode();
                            settings = preset.Value;
                            api("config save", preset.Value);
                        }
                    }
                }
            }
        }
    }
}
