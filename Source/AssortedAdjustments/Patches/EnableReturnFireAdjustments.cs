using System;
using System.Collections.Generic;
using System.Linq;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Levels;
using UnityEngine;

namespace AssortedAdjustments.Patches
{
    internal static class ReturnFireAdjustments
    {
        internal static float reactionAngleCos = (float) Math.Cos(AssortedAdjustments.Settings.ReturnFireAngle * Math.PI / 180d / 2d);
        internal static Dictionary<TacticalActor, int> returnFireCounter = new Dictionary<TacticalActor, int>();



        // Helper
        private static bool CanReturnFireFromAngle(TacticalActor shooter, TacticalActorBase target, float reactionAngleCos)
        {
            // Turrets cannot be flanked
            if (target.IsMetallic)
            {
                return true;
            }

            if (reactionAngleCos > 0.99)
            {
                return true;
            }

            Vector3 targetForward = target.transform.TransformDirection(Vector3.forward);
            Vector3 targetToShooter = (shooter.Pos - target.Pos).normalized;
            float angleCos = Vector3.Dot(targetForward, targetToShooter);

            return Utl.GreaterThanOrEqualTo(angleCos, reactionAngleCos);
        }

        private static bool HasReachedReturnFireLimit(TacticalActor target)
        {
            // Turrets have no limit
            if (target.IsMetallic)
            {
                return false;
            }

            int returnFireLimit = AssortedAdjustments.Settings.ReturnFireLimit;
            if (returnFireLimit > 0)
            {
                returnFireCounter.TryGetValue(target, out var currentCount);
                Logger.Debug($"[ReturnFireAdjustments_HasReachedReturnFireLimit] {target.DisplayName}, returnFireCount: {currentCount}");
                return currentCount >= returnFireLimit;
            }
            else
            {
                return true;
            }
        }



        // Patches
        [HarmonyPatch(typeof(TacticalFaction), "PlayTurnCrt")]
        public static class TacticalFaction_PlayTurnCrt_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments;
            }

            public static void Prefix(TacticalFaction __instance)
            {
                try
                {
                    //Logger.Debug($"[TacticalFaction_PlayTurnCrt_PREFIX] Resetting return fire count for all actors of faction ({__instance.TacticalFactionDef.GetName()}).");
                    /*
                    List<TacticalActor> allFactionActors = __instance.TacticalActors.ToList();
                    foreach (TacticalActor actor in allFactionActors)
                    {
                        if (returnFireCounter.ContainsKey(actor))
                        {
                            returnFireCounter[actor] = 0;
                        }
                        else
                        {
                            returnFireCounter.Add(actor, 0);
                        }
                    }
                    */

                    // Only track enemies of the faction starting its turn
                    returnFireCounter = returnFireCounter
                        .Where(tacticalActor => tacticalActor.Key.TacticalFaction != __instance)
                        .ToDictionary(tacticalActor => tacticalActor.Key, tacticalActor => tacticalActor.Value);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(TacticalLevelController), "FireWeaponAtTargetCrt")]
        public static class TacticalLevelController_FireWeaponAtTargetCrt_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments;
            }

            public static void Prefix(TacticalLevelController __instance, Weapon weapon, TacticalAbilityTarget abilityTarget)
            {
                try
                {
                    if (abilityTarget.AttackType == AttackType.ReturnFire)
                    {
                        TacticalActor tacticalActor = weapon.TacticalActor;
                        returnFireCounter.TryGetValue(tacticalActor, out var currentCount);
                        returnFireCounter[tacticalActor] = currentCount + 1;

                        Logger.Debug($"[TacticalLevelController_FireWeaponAtTargetCrt_PREFIX] Actor: {tacticalActor.DisplayName}, returnFireCount: {returnFireCounter[tacticalActor]}");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(TacticalLevelController), "GetReturnFireAbilities")]
        public static class TacticalLevelController_GetReturnFireAbilities_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments;
            }

            public static void Postfix(TacticalLevelController __instance, ref List<ReturnFireAbility> __result, TacticalActor shooter, Weapon weapon, TacticalAbilityTarget target)
            {
                try
                {
                    // No potential return fire from original method
                    if (__result == null || __result.Count == 0)
                    {
                        Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] Original method already canceled return fire.");
                        return;
                    }

                    // Double check
                    WeaponDef weaponDef = weapon?.WeaponDef;
                    if (target.AttackType == AttackType.ReturnFire || target.AttackType == AttackType.Overwatch || target.AttackType == AttackType.Synced || target.AttackType == AttackType.ZoneControl || (weaponDef != null && weaponDef.NoReturnFireFromTargets))
                    {
                        return;
                    }



                    Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] Disabling return fire if target is shot from outside its reaction angle ({AssortedAdjustments.Settings.ReturnFireAngle} degrees) or has reached its return fire limit ({AssortedAdjustments.Settings.ReturnFireLimit}).");

                    List<ReturnFireAbility> returnFireAbilities = __result;

                    // Reverse iteration to be able to modify list directly
                    for (int i = returnFireAbilities.Count - 1; i >= 0; i--)
                    {
                        TacticalActor tacticalActor = returnFireAbilities[i].TacticalActor;

                        Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] shooter: {shooter.DisplayName}");
                        Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] target: {tacticalActor.DisplayName}");

                        // Check flanked
                        if (!CanReturnFireFromAngle(shooter, tacticalActor, reactionAngleCos))
                        {
                            Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] {tacticalActor.DisplayName} is flanked and cannot return fire.");
                            returnFireAbilities.RemoveAt(i);
                            continue;
                        }
                        else
                        {
                            Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] {tacticalActor.DisplayName} isn't flanked will still return fire.");
                        }

                        // Check shot limit
                        if (HasReachedReturnFireLimit(tacticalActor))
                        {
                            Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] {tacticalActor.DisplayName} has reached its return fire limit and cannot return fire.");
                            returnFireAbilities.RemoveAt(i);
                            continue;
                        }
                        else
                        {
                            Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] {tacticalActor.DisplayName} hasn't reached its return fire limit and will still return fire.");
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
