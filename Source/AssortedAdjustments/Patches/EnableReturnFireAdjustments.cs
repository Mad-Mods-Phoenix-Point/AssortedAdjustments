using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Levels;
using PhoenixPoint.Tactical.View.ViewControllers;
using PhoenixPoint.Tactical.View.ViewStates;
using UnityEngine;
using UnityEngine.UI;

namespace AssortedAdjustments.Patches
{
    internal static class ReturnFireAdjustments
    {
        internal static float reactionAngleCos = (float)Math.Cos(AssortedAdjustments.Settings.ReturnFireAngle * Math.PI / 180d / 2d);
        internal static Dictionary<TacticalActor, int> returnFireCounter = new Dictionary<TacticalActor, int>();
        internal static KeyValuePair<bool, string> stepOutTracker = new KeyValuePair<bool, string>(false, "");



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
                    // Only track enemies of the faction starting its turn
                    returnFireCounter = returnFireCounter.Where(tacticalActor => tacticalActor.Key.TacticalFaction != __instance).ToDictionary(tacticalActor => tacticalActor.Key, tacticalActor => tacticalActor.Value);
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
                    TacticalActor tacticalActor = weapon.TacticalActor;

                    if (abilityTarget.AttackType == AttackType.ReturnFire)
                    {
                        returnFireCounter.TryGetValue(tacticalActor, out var currentCount);
                        returnFireCounter[tacticalActor] = currentCount + 1;

                        Logger.Debug($"[TacticalLevelController_FireWeaponAtTargetCrt_PREFIX] Actor: {tacticalActor.DisplayName}, returnFireCount: {returnFireCounter[tacticalActor]}");
                    }


                    Logger.Debug($"[TacticalLevelController_FireWeaponAtTargetCrt_PREFIX] AttackType: {abilityTarget.AttackType}");
                    if (AssortedAdjustments.Settings.NoReturnFireWhenSteppingOut && abilityTarget.AttackType == AttackType.Regular)
                    {
                        bool shooterStepsOut = Vector3.SqrMagnitude(tacticalActor.Pos - abilityTarget.ShootFromPos) > 0.01f;
                        Logger.Debug($"[TacticalLevelController_FireWeaponAtTargetCrt_PREFIX] shooterStepsOut: {shooterStepsOut}");
                        if (shooterStepsOut)
                        {
                            string msg = $"{tacticalActor.DisplayName} stepped out to shoot with {weapon.DisplayName}.";
                            stepOutTracker = new KeyValuePair<bool, string>(true, msg);
                            Logger.Debug($"[UIStateAbilitySelected_CalculateReturnFirePredictions_PREFIX] {msg}");
                        }
                        else
                        {
                            stepOutTracker = new KeyValuePair<bool, string> (false, "");
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIStateShoot), "CalculateReturnFirePredictions")]
        public static class UIStateShoot_CalculateReturnFirePredictions_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments && AssortedAdjustments.Settings.NoReturnFireWhenSteppingOut;
            }

            public static void Prefix(UIStateShoot __instance)
            {
                try
                {
                    ShootAbility ___shootAbility = (ShootAbility)AccessTools.Property(typeof(UIStateShoot), "_shootAbility").GetValue(__instance, null);

                    if (__instance.AbilityTarget == null || ___shootAbility.Weapon == null)
                    {
                        return;
                    }

                    TacticalActor tacticalActor = ___shootAbility.TacticalActor;
                    TacticalAbilityTarget abilityTarget = __instance.AbilityTarget;

                    Logger.Debug($"[UIStateShoot_CalculateReturnFirePredictions_PREFIX] AttackType: {abilityTarget.AttackType}");
                    if (abilityTarget.AttackType == AttackType.Regular)
                    {
                        bool shooterWillStepOut = Vector3.SqrMagnitude(tacticalActor.Pos - abilityTarget.ShootFromPos) > 0.01f;
                        Logger.Debug($"[UIStateShoot_CalculateReturnFirePredictions_PREFIX] shooterWillStepOut: {shooterWillStepOut}");
                        if (shooterWillStepOut)
                        {
                            string msg =  $"{tacticalActor.DisplayName} will step out to shoot with {___shootAbility.Weapon.DisplayName}.";
                            stepOutTracker = new KeyValuePair<bool, string>(true, msg);
                            Logger.Debug($"[UIStateAbilitySelected_CalculateReturnFirePredictions_PREFIX] {msg}");
                        }
                        else
                        {
                            stepOutTracker = new KeyValuePair<bool, string>(false, "");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIStateAbilitySelected), "CalculateReturnFirePredictions")]
        public static class UIStateAbilitySelected_CalculateReturnFirePredictions_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments && AssortedAdjustments.Settings.NoReturnFireWhenSteppingOut;
            }

            public static void Prefix(UIStateAbilitySelected __instance, List<TacticalActorBase> ____targetActors, TacticalAbility ____selectedAbility, bool ____groundTargeting)
            {
                try
                {
                    if (!____targetActors.Any<TacticalActorBase>() || __instance.SelectedAbilityTarget == null || !(____selectedAbility is IAttackAbility))
                    {
                        return;
                    }

                    if (____groundTargeting)
                    {
                        Logger.Debug($"[UIStateAbilitySelected_CalculateReturnFirePredictions_PREFIX] Ability is ground-targeting. Predictions regarding side-stepping may be inaccurate!");
                        //return;
                    }

                    TacticalActor tacticalActor = ____selectedAbility.TacticalActor;
                    TacticalAbilityTarget abilityTarget = __instance.SelectedAbilityTarget;

                    Logger.Debug($"[UIStateAbilitySelected_CalculateReturnFirePredictions_PREFIX] AttackType: {abilityTarget.AttackType}");
                    if (abilityTarget.AttackType == AttackType.Regular)
                    {
                        bool performerWillStepOut = Vector3.SqrMagnitude(tacticalActor.Pos - abilityTarget.ShootFromPos) > 0.01f;
                        Logger.Debug($"[UIStateAbilitySelected_CalculateReturnFirePredictions_PREFIX] performerWillStepOut: {performerWillStepOut}");
                        if (performerWillStepOut)
                        {
                            string msg = $"{tacticalActor.DisplayName} will step out to use {____selectedAbility.TacticalAbilityDef?.ViewElementDef?.DisplayName1.Localize()} ({____selectedAbility.TargetEquipmentName}).";
                            stepOutTracker = new KeyValuePair<bool, string>(true, msg);
                            Logger.Debug($"[UIStateAbilitySelected_CalculateReturnFirePredictions_PREFIX] {msg}");
                        }
                        else
                        {
                            stepOutTracker = new KeyValuePair<bool, string>(false, "");
                        }
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
                        Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] Original method ruled out return fire.");
                        return;
                    }

                    // Double check
                    WeaponDef weaponDef = weapon?.WeaponDef;
                    if (target.AttackType == AttackType.ReturnFire || target.AttackType == AttackType.Overwatch || target.AttackType == AttackType.Synced || target.AttackType == AttackType.ZoneControl || (weaponDef != null && weaponDef.NoReturnFireFromTargets))
                    {
                        return;
                    }



                    Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] Checking additional limitations for return fire.");

                    List<ReturnFireAbility> returnFireAbilities = __result;
                    for (int i = returnFireAbilities.Count - 1; i >= 0; i--) // Reverse iteration to be able to modify list directly
                    {
                        TacticalActor tacticalActor = returnFireAbilities[i].TacticalActor;

                        Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] shooter: {shooter.DisplayName}");
                        Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] target: {tacticalActor.DisplayName}");

                        // Check for step out
                        if(AssortedAdjustments.Settings.NoReturnFireWhenSteppingOut && stepOutTracker.Key == true)
                        {
                            Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] {tacticalActor.DisplayName} cannot return fire because {stepOutTracker.Value}");
                            returnFireAbilities.RemoveAt(i);
                            continue;
                        }

                        // Check flanked
                        if (!CanReturnFireFromAngle(shooter, tacticalActor, reactionAngleCos))
                        {
                            Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] {tacticalActor.DisplayName} is flanked and cannot return fire.");
                            returnFireAbilities.RemoveAt(i);
                            continue;
                        }

                        // Check shot limit
                        if (HasReachedReturnFireLimit(tacticalActor))
                        {
                            Logger.Debug($"[TacticalLevelController_GetReturnFireAbilities_POSTFIX] {tacticalActor.DisplayName} has reached its return fire limit and cannot return fire.");
                            returnFireAbilities.RemoveAt(i);
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(SpottedTargetsElement), "ShowReturnFireIcon")]
        public static class SpottedTargetsElement_ShowReturnFireIcon_Patch
        {
            private static List<SpottedTargetsElement> AlreadyAdjustedElements = new List<SpottedTargetsElement>();

            private static Color lerpedColor = Color.white;
            private static float t = 0f;
            private static bool up = true;
            private static readonly float step = 0.03f;

            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments && AssortedAdjustments.Settings.EmphasizeReturnFireHint;
            }

            private static IEnumerator Pulse(Image i, Color c1, Color c2)
            {
                while (true)
                {
                    if (up && t < 1f)
                    {
                        t += step;
                    }
                    else if (up && t >= 1f)
                    {
                        t -= step;
                        up = false;
                    }
                    else if (t > 0)
                    {
                        t -= step;
                    }
                    else if (t <= 0)
                    {
                        t += step;
                        up = true;
                    }

                    lerpedColor = Color.Lerp(c1, c2, t);
                    i.color = lerpedColor;

                    yield return new WaitForSeconds(1/30);
                }
            }

            public static void Prefix(SpottedTargetsElement __instance)
            {
                try
                {
                    if (!AlreadyAdjustedElements.Contains(__instance))
                    {
                        Logger.Debug($"[SpottedTargetsElement_ShowReturnFireIcon_PREFIX] Rescale and reposition return fire icon");
                        __instance.ReturnFire.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
                        __instance.ReturnFire.transform.Translate(new Vector3(-2f, 3f, 1f));
                        AlreadyAdjustedElements.Add(__instance);
                    }

                    //__instance.StopCoroutine("Pulse");
                    __instance.StopAllCoroutines();
                    __instance.StartCoroutine(Pulse(__instance.ReturnFire, Color.white, Color.red));
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }









        /*
        // @ToDo: Try to adjust return fire for steeping out
        [HarmonyPatch(typeof(TacticalLevelController), "ShootAndWaitRF")]
        public static class TacticalLevelController_ShootAndWaitRF_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments;
            }

            public static void Postfix(TacticalLevelController __instance, TacticalActor tacticalActor, TacticalAbilityTarget abilityTarget)
            {
                try 
                { 
                    Logger.Debug($"[TacticalLevelController_ShootAndWaitRF_POSTFIX] Called.");
                    Logger.Debug($"[TacticalLevelController_ShootAndWaitRF_POSTFIX] tacticalActor: {tacticalActor.DisplayName}");
                    Logger.Debug($"[TacticalLevelController_ShootAndWaitRF_POSTFIX] abilityTarget: {abilityTarget.Actor.DisplayName}");

                    // Reverse combatants
                    //TacticalActor newTarget = tacticalActor;

                    TacticalActor newShooter = abilityTarget.Actor as TacticalActor;

                    TacticalAbility defaultUseAbilityForSelectedEquipment = newShooter.GetDefaultUseAbilityForSelectedEquipment("SetShootAbility");
                    ShootAbility shootAbility = defaultUseAbilityForSelectedEquipment as ShootAbility;
                    Weapon weapon = shootAbility.Weapon;

                    TacticalAbilityTarget newTarget = new TacticalAbilityTarget(tacticalActor);

                    Logger.Info($"[TacticalLevelController_ShootAndWaitRF_POSTFIX] Call FireWeaponAtTargetCrt({weapon.DisplayName}, {newTarget.Actor.DisplayName}, {shootAbility.TacticalAbilityDef.name})");

                    //__instance.FireWeaponAtTargetCrt(weapon, newTarget, shootAbility);
                    __instance.Timing.Start(__instance.FireWeaponAtTargetCrt(weapon, newTarget, shootAbility), NextUpdate.ThisFrame, false, null);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(TacticalLevelController), "ReturnFire")]
        public static class TacticalLevelController_ReturnFire_Patch
        {
            private static readonly IgnoredAbilityDisabledStatesFilter ReactiveShotFilter = new IgnoredAbilityDisabledStatesFilter(new AbilityDisabledState[]
            {
                AbilityDisabledState.NoMoreUsesThisTurn,
                AbilityDisabledState.NotEnoughActionPoints,
                AbilityDisabledState.NoValidTarget,
                AbilityDisabledState.EquipmentNotSelected,
                AbilityDisabledState.RequirementsNotMet
            });

            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableReturnFireAdjustments;
            }

            public static bool Prefix()
            {
                return false;
            }

            // Complete rebuild of the original method
            public static IEnumerator<NextUpdate> Postfix(IEnumerator<NextUpdate> passThrough, TacticalLevelController __instance, TacticalActor shooter, Weapon weapon, TacticalAbilityTarget target, ShootAbility shootAbility, List<TacticalActor> casualties, Func<ReturnFireAbility, bool> validReturnFireAbilityPredicate)
            {
                if (!__instance.IsReturnFireEnabled)
                {
                    yield break;
                }
                while (__instance.Map.HasActiveProjectiles)
                {
                    yield return NextUpdate.NextFrame;
                }
                List<ReturnFireAbility> returnFireAbilities2 = __instance.GetReturnFireAbilities(shooter, weapon, target, shootAbility, true, casualties);
                List<ReturnFireAbility> returnFireAbilities = (returnFireAbilities2 != null) ? returnFireAbilities2.Where(validReturnFireAbilityPredicate).ToList<ReturnFireAbility>() : null;
                List<ReturnFireAbility> list = returnFireAbilities;
                if (list == null || list.Count == 0)
                {
                    yield break;
                }
                shooter.TimingScale.AddScale(__instance.ReturnFireTimeScale, __instance);
                yield return __instance.Timing.Call(__instance.View.CameraDirector.WaitWhileBusy(), null);
                yield return NextUpdate.Seconds(0.5f * shooter.Timing.CumulativeScale);
                TimingScheduler.CurrentScheduler.CurrentUpdateable.OnStop += delegate (IUpdateable u)
                {
                    shooter.TimingScale.RemoveScale(__instance.ReturnFireTimeScale, __instance);
                };
                __instance.View.SetOverwatchVisuals(false, false, false);
                foreach (ReturnFireAbility returnFireAbility in returnFireAbilities)
                {
                    TacticalAbility retaliationAbility = returnFireAbility.GetRetaliationAbility() as TacticalAbility;
                    TacticalAbility tacticalAbility = retaliationAbility;
                    if (tacticalAbility != null && tacticalAbility.IsEnabled(ReactiveShotFilter))
                    {
                        TacticalAbilityTarget returnFireTarget = ((IAttackAbility)retaliationAbility).GetAttackActorTarget(shooter, AttackType.ReturnFire);
                        if (returnFireTarget != null)
                        {
                            yield return __instance.Timing.Call(__instance.View.CameraDirector.WaitWhileBusy(), null);
                            yield return __instance.Timing.Call(retaliationAbility.Execute(returnFireTarget), null);
                            retaliationAbility = null;
                            returnFireTarget = null;
                        }
                    }
                }

                List<ReturnFireAbility>.Enumerator enumerator = default(List<ReturnFireAbility>.Enumerator);
                __instance.View.SetOverwatchVisuals(false, false, true);
                yield return __instance.Timing.Call(__instance.View.CameraDirector.WaitWhileBusy(), null);
                yield break;
                yield break;
            }
        }
        */
    }
}
