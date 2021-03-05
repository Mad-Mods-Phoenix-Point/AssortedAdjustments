using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using System;

namespace AssortedAdjustments.LimitedWar
{
    // Strengthen some factions and multiply defenses by config and alertness level
    [HarmonyPatch(typeof(GeoHavenDefenseMission), "GetDefenseDeployment")]
    public static class GeoHavenDefenseMission_GetDefenseDeployment_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && Config.DefenseMultipliers.Enabled;
        }

        public static void Postfix(GeoHavenDefenseMission __instance, ref int __result, GeoHaven haven)
        {
            try
            {
                if (haven == null)
                {
                    return;
                }

                GeoFaction attacker = __instance.GetEnemyFaction() is GeoSubFaction sub ? attacker = sub.BaseFaction : __instance.GetEnemyFaction() as GeoFaction;
                GeoFaction defender = haven.Site.Owner;
                Logger.Info($"{haven.Site.Name} ({defender}) under attack by {attacker.GetPPName()}. Defense strength: {__result}. Alertness: {haven.AlertLevel}");

                DefenseMultipliers defenseMultipliers = Config.DefenseMultipliers;
                float multiply = defenseMultipliers.Default;
                switch (haven?.AlertLevel)
                {
                    case GeoHaven.HavenAlertLevel.Alert:
                        multiply *= defenseMultipliers.Alert;
                        break;
                    case GeoHaven.HavenAlertLevel.HighAlert:
                        multiply *= defenseMultipliers.HighAlert;
                        break;
                }

                GeoLevelController geoLevel = haven.Site.GeoLevel;

                if (Resolver.IsAlien(attacker))
                {
                    multiply *= defenseMultipliers.AttackerPandora;
                }
                else if (attacker == geoLevel.AnuFaction)
                {
                    multiply *= defenseMultipliers.AttackerAnu;
                }
                else if (attacker == geoLevel.SynedrionFaction)
                {
                    multiply *= defenseMultipliers.AttackerSynedrion;
                }
                else if (attacker == geoLevel.NewJerichoFaction)
                {
                    multiply *= defenseMultipliers.AttackerNewJericho;
                }

                if (Resolver.IsAlien(defender))
                {
                    multiply *= defenseMultipliers.DefenderPandora;
                }
                else if (defender == geoLevel.AnuFaction)
                {
                    multiply *= defenseMultipliers.DefenderAnu;
                }
                else if (defender == geoLevel.SynedrionFaction)
                {
                    multiply *= defenseMultipliers.DefenderSynedrion;
                }
                else if (defender == geoLevel.NewJerichoFaction)
                {
                    multiply *= defenseMultipliers.DefenderNewJericho;
                }

                if (multiply != 1)
                {
                    __result = (int)Math.Round(__result * multiply);
                    Logger.Info($"Multiplying defender strength by {multiply} to {__result}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}