using System;
using System.Linq;
using Base.Core;
using Harmony;
using PhoenixPoint.Geoscape.Entities.Abilities;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewStates;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(UIStateVehicleSelected), "AddTravelSite")]
    public static class UIStateVehicleSelected_AddTravelSite_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.PauseOnDestinationSet;
        }

        public static void Prefix(UIStateVehicleSelected __instance, ref bool __state)
        {
            try
            {
                GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                __state = _GeoscapeViewContext.Level.Timing.Paused;

                Logger.Debug($"[UIStateVehicleSelected_AddTravelSite_PREFIX] Current time setting: {(__state ? "Paused" : "Running")}.");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Postfix(UIStateVehicleSelected __instance, ref bool __state)
        {
            try
            {
                Logger.Debug($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] Called.");

                int craftCount = GameUtl.CurrentLevel().GetComponent<GeoLevelController>().ViewerFaction.Vehicles.Count();
                Logger.Info($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] craftCount: {craftCount}");

                if (craftCount > 1)
                {
                    Logger.Debug($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] New vehicle travel plan. Keeping time setting at: {(__state ? "Paused" : "Running")}.");

                    GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                    _GeoscapeViewContext.Level.Timing.Paused = __state;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    [HarmonyPatch(typeof(UIStateVehicleSelected), "OnContextualItemSelected")]
    public static class UIStateVehicleSelected_OnContextualItemSelected_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.PauseOnDestinationSet;
        }

        public static void Prefix(UIStateVehicleSelected __instance, ref bool __state)
        {
            try
            {
                GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                __state = _GeoscapeViewContext.Level.Timing.Paused;

                Logger.Debug($"[UIStateVehicleSelected_OnContextualItemSelected_PREFIX] Current time setting: {(__state ? "Paused" : "Running")}.");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Postfix(UIStateVehicleSelected __instance, GeoAbility ability, ref bool __state)
        {
            try
            {
                Logger.Debug($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] Called.");

                if (!(ability is MoveVehicleAbility))
                {
                    return;
                }

                int craftCount = GameUtl.CurrentLevel().GetComponent<GeoLevelController>().ViewerFaction.Vehicles.Count();
                Logger.Info($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] craftCount: {craftCount}");

                if (craftCount > 1)
                {
                    Logger.Debug($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] New vehicle travel plan. Keeping time setting at: {(__state ? "Paused" : "Running")}.");

                    GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                    _GeoscapeViewContext.Level.Timing.Paused = __state;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
