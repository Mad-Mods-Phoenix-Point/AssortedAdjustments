using Base.UI;
using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Missions;
using PhoenixPoint.Geoscape.Levels.Objectives;
using PhoenixPoint.Geoscape.View.ViewModules;
using System;
using UnityEngine;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    // Show additional info in objectives
    [HarmonyPatch(typeof(UIModuleGeoObjectives), "InitObjective")]
    public static class UIModuleGeoObjectives_InitObjective_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.BetterObjectives;
        }

        public static void Prefix(UIModuleGeoObjectives __instance, ref GeoFactionObjective objective)
        {
            try
            {
                if(!(objective is MissionGeoFactionObjective missionGeoFactionObjective) || !(missionGeoFactionObjective.Mission is GeoHavenDefenseMission geoHavenDefenseMission))
                {
                    return;
                }

                IGeoFactionMissionParticipant enemyFaction = geoHavenDefenseMission.GetEnemyFaction();
                Color enemyColor = enemyFaction.ParticipantViewDef.FactionColor;
                string enemyColorHex = $"#{ColorUtility.ToHtmlStringRGB(enemyColor)}";
                string enemyName = enemyFaction.ParticipantName.Localize();
                string enemyText = $"<color={enemyColorHex}>{enemyName}</color>";

                objective.Title = new LocalizedTextBind("Defend {0} against " + enemyText, true);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}