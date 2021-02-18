using System;
using Harmony;
using PhoenixPoint.Tactical.Entities.Statuses;

namespace AssortedAdjustments.Patches
{
    class ModifyDamageOverTimeStatus
    {
        [HarmonyPatch(typeof(DamageOverTimeStatus), "LowerDamageOverTimeLevel")]
        public static class DamageOverTimeStatus_LowerDamageOverTimeLevel_Patch
        {
            public static void Prefix(DamageOverTimeStatus __instance, ref float amount)
            {
                try
                {
                    if (__instance is ParalysisDamageOverTimeStatus || __instance is InfectedStatus)
                    {
                        Logger.Debug($"[DamageOverTimeStatus_LowerDamageOverTimeLevel_PREFIX] [PARALYSIS || INFECTED] amount: {amount}");

                        if ((bool)(__instance.TacticalActor?.IsControlledByPlayer))
                        {
                            amount *= 10f;
                        }
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
