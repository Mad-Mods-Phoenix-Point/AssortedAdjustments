using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Utils;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewStates;
using UnityEngine;

namespace AssortedAdjustments.Patches
{
    internal static class SmartBaseSelection
    {
        private static GeoPhoenixBase GetClosestPhoenixBaseFromScreenCenter(GeoscapeViewContext context, bool perimeterSelection = true, float perimeterSize = 1000f)
        {
            // Get current screen center and cast a ray onto the globe
            // Get the object at the hit position (optionally search a radius around that point and collect all sites)
            // Determine if there is an active phoenix base around and return it
            // REF: GeoscapeView.SelectAtCursor() and GeoMap.PickObjectOnPosition()

            GeoSite geoSite = null;
            GeoPhoenixBase geoPhoenixBase = null;

            Vector3 centerScreenPos = context.CameraDirector.Manager.CenterScreenPos;
            Ray ray = context.CameraDirector.Camera.ScreenPointToRay(centerScreenPos);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 10000f, UnityLayers.Globe))
            {
                Vector3 globePos = raycastHit.point;

                if (perimeterSelection)
                {
                    float cameraFov = context.View.InteractableFoV.Evaluate(context.CameraDirector.Camera.fieldOfView);
                    float searchRange = Mathf.Clamp(perimeterSize, cameraFov, 2000f);
                    EarthUnits surfaceRange = new EarthUnits(searchRange);
                    IEnumerable<GeoSite> sitesInRange = GeoMap.GetSitesInRange(globePos, surfaceRange, true);

                    Logger.Info($"[SmartBaseSelection_GetClosestPhoenixBaseFromScreenCenter] cameraFov: {cameraFov}");
                    Logger.Info($"[SmartBaseSelection_GetClosestPhoenixBaseFromScreenCenter] surfaceRange: {surfaceRange}");
                    Logger.Info($"[SmartBaseSelection_GetClosestPhoenixBaseFromScreenCenter] sitesInRange: {String.Join(", ", sitesInRange.Select(s => s.Name ?? s.name).ToArray())}");

                    if (sitesInRange.Count() > 0)
                    {
                        geoSite = sitesInRange.Where(s => s.Type == GeoSiteType.PhoenixBase && s.State == GeoSiteState.Functioning).FirstOrDefault();
                    }
                }
                else
                {
                    geoSite = context.Map.PickObjectOnPosition<GeoSite>(globePos, false);
                }
            }

            if (geoSite != null && geoSite.Type == GeoSiteType.PhoenixBase && geoSite.State == GeoSiteState.Functioning)
            {
                geoPhoenixBase = geoSite.GetComponent<GeoPhoenixBase>();
                Logger.Debug($"[UIStatePhoenixBaseLayout_EnterState_PREFIX] raycastSelector returned geoPhoenixBase: {geoPhoenixBase.Site.Name}");
            }

            return geoPhoenixBase;
        }



        [HarmonyPatch(typeof(UIStatePhoenixBaseLayout), "EnterState")]
        public static class UIStatePhoenixBaseLayout_EnterState_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableSmartBaseSelection;
            }

            public static void Prefix(UIStatePhoenixBaseLayout __instance, ref GeoPhoenixBase ____base, ref bool ____newBaseUI)
            {
                try
                {
                    if (____base != null) {
                        // Base is already set by selecting base info from context menu

                        return;
                    }
                    else
                    {
                        // Base is null when selected from section menu. 
                        // Original method then tries to get the last shown base.
                        // If not found it defaults to the first base of all bases.
                        // NOW, if we have a more relevant base to select (the closest base to the current screen center, when an aircraft arrived or the base selection module was triggered) we 'll try to get that...

                        GeoscapeViewContext ___Context = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                        ____base = GetClosestPhoenixBaseFromScreenCenter(___Context);
                        Logger.Debug($"[UIStatePhoenixBaseLayout_EnterState_PREFIX] Base: {____base?.Site.Name}");

                        // Force base layout rebuild?
                        ____newBaseUI = true;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
