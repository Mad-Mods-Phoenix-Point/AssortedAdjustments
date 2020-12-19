using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewStates;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(GeoscapeLog), "ProcessQueuedEvents")]
    public static class GeoscapeLog_ProcessQueuedEvents_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.PauseOnHealed;
        }

        public static void Prefix(GeoscapeLog __instance, List<IGeoCharacterContainer> ____justRestedContainer, GeoLevelController ____level)
        {
            try
            {
                if (____justRestedContainer.Count <= 0)
                {
                    return;
                }

                // Some IGeoCharacterContainer was just fully healed
                ____level.View.RequestGamePause();


                // Center?
                if (!AssortedAdjustments.Settings.CenterOnHealed)
                {
                    return;
                }

                // Centering on base or vehicle
                GeoSite geoSite = ____justRestedContainer?.OfType<GeoSite>().FirstOrDefault();
                GeoVehicle geoVehicle = ____justRestedContainer?.OfType<GeoVehicle>().FirstOrDefault();

                // Crew in vehicle
                if (geoVehicle != null)
                {
                    GeoscapeViewState geoscapeViewState = ____level.View.CurrentViewState;
                    Logger.Info($"[GeoscapeLog_ProcessQueuedEvents_PREFIX] Crew on {geoVehicle.Name} is rested. ViewState is {geoscapeViewState}.");

                    if (geoscapeViewState is UIStateVehicleSelected)
                    {
                        Logger.Info($"[GeoscapeLog_ProcessQueuedEvents_PREFIX] Trying to select {geoVehicle.Name}.");
                        typeof(UIStateVehicleSelected).GetMethod("SelectVehicle", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(geoscapeViewState, new object[] { geoVehicle, true });
                    }
                }
                // Crew in base
                else if (geoSite != null)
                {
                    Logger.Info($"[GeoscapeLog_ProcessQueuedEvents_PREFIX] Crew in base {geoSite.Name} is rested. Centering.");
                    ____level.View.ChaseTarget(geoSite, false);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
