using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Missions;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using System;

namespace AssortedAdjustments.LimitedWar
{
    // Utility class for deciding things
    internal static class Resolver
    {
        internal static bool IsAlien(IGeoFactionMissionParticipant f) => f is GeoAlienFaction;
        internal static bool IsPhoenix(IGeoFactionMissionParticipant f) => f is GeoPhoenixFaction;
        internal static bool IsAlienOrPhoenix(IGeoFactionMissionParticipant f) => IsAlien(f) || IsPhoenix(f);



        internal static bool IsLimitedToZoneDamage(IGeoFactionMissionParticipant attacker)
        {
            return !IsPhoenix(attacker) && (Config.LimitPandoranAttacksToZones && IsAlien(attacker)) || (Config.LimitFactionAttacksToZones && !IsAlien(attacker));
        }

        internal static bool CanDestroyHavens(IGeoFactionMissionParticipant attacker)
        {
            return !IsLimitedToZoneDamage(attacker);
        }



        internal static bool HasReachedAttackLimits(GeoLevelController geoLevel, IGeoFactionMissionParticipant attacker)
        {
            try
            {
                if (geoLevel?.Map == null || Resolver.IsAlienOrPhoenix(attacker))
                {
                    return false;
                }

                int havensUnderAttack = 0;
                int havensUnderAttackByPandorans = 0;
                int havensUnderAttackByFactions = 0;
                int ownHavensUnderAttackByPandorans = 0;
                int havensUnderAttackByOwnFaction = 0;

                foreach (GeoSite geoSite in geoLevel.Map.AllSites)
                {
                    if (geoSite.ActiveMission is GeoHavenDefenseMission geoHavenDefenseMission)
                    {
                        havensUnderAttack++;

                        if (Resolver.IsAlien(geoHavenDefenseMission.GetEnemyFaction()))
                        {
                            havensUnderAttackByPandorans++;

                            if (geoSite.Owner == attacker)
                            {
                                ownHavensUnderAttackByPandorans++;
                            }
                        }
                        else
                        {
                            havensUnderAttackByFactions++;
                        }

                        if (geoHavenDefenseMission.GetEnemyFaction() == attacker)
                        {
                            havensUnderAttackByOwnFaction++;
                        }
                    }
                }
                //Logger.Info($"havensUnderAttack: {havensUnderAttack}");
                //Logger.Info($"havensUnderAttackByPandorans: {havensUnderAttackByPandorans}");
                //Logger.Info($"havensUnderAttackByFactions: {havensUnderAttackByFactions}");
                //Logger.Info($"ownHavensUnderAttackByPandorans: {ownHavensUnderAttackByPandorans}");
                //Logger.Info($"havensUnderAttackByOwnFaction: {havensUnderAttackByOwnFaction}");

                if (Config.StopAttacksWhileDefendingPandoransThreshold.Key && ownHavensUnderAttackByPandorans >= Config.StopAttacksWhileDefendingPandoransThreshold.Value)
                {
                    Logger.Info($"Defending pandoran threshold reached ({ownHavensUnderAttackByPandorans}/{Config.StopAttacksWhileDefendingPandoransThreshold.Value}). {attacker.ParticipantName.Localize()} should cancel further attacks.");
                    return true;
                }
                if (Config.GlobalAttackLimit.Key && havensUnderAttackByFactions >= Config.GlobalAttackLimit.Value)
                {
                    Logger.Info($"Global faction attack limit reached ({havensUnderAttackByFactions}/{Config.GlobalAttackLimit.Value}). {attacker.ParticipantName.Localize()} should cancel further attacks.");
                    return true;
                }
                if (Config.FactionAttackLimit.Key && havensUnderAttackByOwnFaction >= Config.FactionAttackLimit.Value)
                {
                    Logger.Info($"Attackers faction attack limit reached ({havensUnderAttackByOwnFaction}/{Config.FactionAttackLimit.Value}). {attacker.ParticipantName.Localize()} should cancel further attacks.");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }



        internal static bool ShouldCancelAttack(GeoLevelController geoLevel, IGeoFactionMissionParticipant attacker)
        {
            try
            {
                if (Config.StopOneSidedWar && Store.LastAttacker != null && attacker == Store.LastAttacker)
                {
                    Logger.Info($"Stopping one sided war. {attacker.GetPPName()} was the most recent aggressor.");
                    return true;
                }
                return HasReachedAttackLimits(geoLevel, attacker);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }
    }
}