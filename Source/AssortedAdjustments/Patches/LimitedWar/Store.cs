using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Missions;

namespace AssortedAdjustments.LimitedWar
{
    internal static class Store
    {
        internal static int GameDifficulty = 1; // Default to Vet if anything goes wrong
        internal static IGeoFactionMissionParticipant LastAttacker = null;
        internal static GeoHavenDefenseMission DefenseMission = null;
    }

    // Store mission for other patches
    [HarmonyPatch(typeof(GeoHavenDefenseMission), "UpdateGeoscapeMissionState")]
    public static class GeoHavenDefenseMission_UpdateGeoscapeMissionState_Patch
    {
        public static bool Prepare()
        {
            //return Config.LimitFactionAttacksToZones || Config.LimitPandoranAttacksToZones;
            return Config.Enable;
        }

        public static void Prefix(GeoHavenDefenseMission __instance)
        {
            Store.DefenseMission = __instance;
        }

        public static void Postfix()
        {
            Store.DefenseMission = null;
        }
    }
}