using Harmony;
using PhoenixPoint.Geoscape.Levels.Factions;

namespace AssortedAdjustments.LimitedWar
{
    // Disable phoenix base attacks from pandorans
    [HarmonyPatch(typeof(GeoAlienFaction), "AttackPhoenixBase")]
    public static class GeoAlienFaction_AttackPhoenixBase_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && Config.DisablePandoranAttacksOnPhoenixBases;
        }

        public static bool Prefix(GeoAlienFaction __instance)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GeoAlienFaction), "StartPhoenixBaseAssault")]
    public static class GeoAlienFaction_StartPhoenixBaseAssault_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && Config.DisablePandoranAttacksOnPhoenixBases;
        }

        public static bool Prefix(GeoAlienFaction __instance)
        {
            return false;
        }
    }
}