using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssortedAdjustments.LimitedWar
{
    // Limit factions attacks on havens
    [HarmonyPatch(typeof(GeoFaction), "AttackHavenFromVehicle")]
    public static class GeoFaction_AttackHavenFromVehicle_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && Config.HasAttackLimitsActive;
        }

        // Override
        public static bool Prefix(GeoFaction __instance, GeoVehicle vehicle, GeoSite site, GeoLevelController ____level)
        {
            try
            {
                Logger.Info($"{vehicle.Name} wants to attack {site.Name}.");
                if (Resolver.ShouldCancelAttack(____level, vehicle?.Owner))
                {
                    Logger.Info($"{__instance.Name.Localize()} discouraged from attacking {site.Name}.");
                    return false;
                }
                Store.LastAttacker = vehicle.Owner;
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(GeoLevelController), "OnLevelStart")]
    public static class GeoLevelController_OnLevelStart_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && Config.HasAttackLimitsActive;
        }

        public static void Postfix(GeoLevelController __instance)
        {
            try
            {
                GameDifficultyLevelDef diff = __instance.CurrentDifficultyLevel;
                Store.GameDifficulty = __instance.DynamicDifficultySystem.DifficultyLevels.ToList().IndexOf(diff);
                Store.LastAttacker = null;
                Logger.Info($"Last attacker was reset. Game's difficulty level is {Store.GameDifficulty}."); // 0 = Rookie, 1 = Veteran, 2 = Hero, 3 = Legend

                if (Config.UseDifficultyDrivenLimits)
                {
                    int d = Store.GameDifficulty;
                    Config.StopAttacksWhileDefendingPandoransThreshold = new KeyValuePair<bool, int>(true, d + 1);
                    Config.GlobalAttackLimit = new KeyValuePair<bool, int>(true, d + 2);
                    Config.FactionAttackLimit = new KeyValuePair<bool, int>(true, d + 1);

                    Logger.Info($"Static limits overridden with difficulty driven values");
                    Logger.Info($"StopAttacksWhileDefendingPandoransThreshold: {Config.StopAttacksWhileDefendingPandoransThreshold.Key}, {Config.StopAttacksWhileDefendingPandoransThreshold.Value}");
                    Logger.Info($"GlobalAttackLimit: {Config.GlobalAttackLimit.Key}, {Config.GlobalAttackLimit.Value}");
                    Logger.Info($"FactionAttackLimit: {Config.FactionAttackLimit.Key}, {Config.FactionAttackLimit.Value}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    // Formerly VehicleFactionController.UpdateNavigation
    [HarmonyPatch(typeof(VehicleFactionController), "GetSiteVehicleDestinationWeight")]
    public static class VehicleFactionController_GetSiteVehicleDestinationWeight_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && Config.HasAttackLimitsActive;
        }

        public static void Prefix(VehicleFactionController __instance, ref float? __state)
        {
            try
            {
                if (Resolver.ShouldCancelAttack(__instance.Vehicle?.GeoLevel, __instance.Vehicle?.Owner))
                {
                    Logger.Info($"Discouraging {__instance.Vehicle.Name} from faction war.");
                    __state = __instance.ControllerDef.FactionInWarWeightMultiplier;
                    __instance.ControllerDef.FactionInWarWeightMultiplier = -2f;
                }
                else
                {
                    __state = null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Postfix(VehicleFactionController __instance, float? __state)
        {
            if (__state != null)
            {
                __instance.ControllerDef.FactionInWarWeightMultiplier = __state.Value;
            }
        }
    }
}