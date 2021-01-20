using System;
using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewStates;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(UIStateVehicleSelected), "OnVehicleArrived")]
    public static class UIStateVehicleSelected_OnVehicleArrived_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.CenterOnVehicleArrived;
        }

        public static void Prefix(UIStateVehicleSelected __instance, GeoVehicle vehicle, bool justPassing)
        {
            try
            {
                GeoVehicle ___SelectedVehicle = (GeoVehicle)AccessTools.Property(typeof(UIStateVehicleSelected), "SelectedVehicle").GetValue(__instance, null);

                if (justPassing || vehicle != ___SelectedVehicle)
                {
                    return;
                }

                // Vehicle IS selected but probably not in viewport
                GeoscapeViewContext ___Context = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                ___Context.View.ChaseTarget(vehicle, false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    // General "bugfix" for opening the context menu at wrong position in UIStateVehicleSelected.OnVehicleArrived() -> disable this senseless s&?t completely
    [HarmonyPatch(typeof(UIStateVehicleSelected), "ShouldShowContextMenuOnArrivate")] // (sic!)
    public static class UIStateVehicleSelected_ShouldShowContextMenuOnArrivate_Patch
    {
        public static bool Prefix(UIStateVehicleSelected __instance)
        {
            return false;
        }
    }
}
