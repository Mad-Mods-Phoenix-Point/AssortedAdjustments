using System;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Entities.Addons;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.View.ViewControllers.AugmentationScreen;
using PhoenixPoint.Geoscape.View.ViewModules;
using UnityEngine;
using UnityEngine.UI;

namespace AssortedAdjustments.Patches
{
    internal static class SoldierAdjustments
    {
        internal static int MaxAugmentations = Mathf.Clamp(AssortedAdjustments.Settings.MaxAugmentations, 0, 3);



        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<BaseStatSheetDef> baseStatSheetDefs = defRepository.DefRepositoryDef.AllDefs.OfType<BaseStatSheetDef>().ToList();
            foreach (BaseStatSheetDef bssDef in baseStatSheetDefs)
            {
                bssDef.PersonalAbilitiesCount = AssortedAdjustments.Settings.PersonalAbilitiesCount;
                bssDef.MaxStrength = AssortedAdjustments.Settings.MaxStrength;
                bssDef.MaxWill = AssortedAdjustments.Settings.MaxWill;
                bssDef.MaxSpeed = AssortedAdjustments.Settings.MaxSpeed;

                Logger.Info($"[SoldierAdjustments_Apply] bssDef: {bssDef.name}, GUID: {bssDef.Guid}, PersonalAbilitiesCount: {bssDef.PersonalAbilitiesCount}, Attributes: {bssDef.MaxStrength}, {bssDef.MaxWill}, {bssDef.MaxSpeed}");
            }
        }



        // Patches
        [HarmonyPatch(typeof(UIModuleMutate), "InitCharacterInfo")]
        public static class UIModuleMutate_InitCharacterInfo_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableSoldierAdjustments;
            }

            public static bool Prefix(UIModuleMutate __instance, ref int ____currentCharacterAugmentsAmount, Dictionary<AddonSlotDef, UIModuleMutationSection> ____augmentSections, GameTagDef ____bionicsTag, GameTagDef ____mutationTag)
            {
                try
                {
                    ____currentCharacterAugmentsAmount = 0;
                    ____currentCharacterAugmentsAmount = AugmentScreenUtilities.GetNumberOfAugments(__instance.CurrentCharacter);
                    bool flag = ____currentCharacterAugmentsAmount < MaxAugmentations;
                    foreach (KeyValuePair<AddonSlotDef, UIModuleMutationSection> keyValuePair in ____augmentSections)
                    {
                        AugumentSlotState slotState = AugumentSlotState.Available;
                        string lockedReasonKey = null;
                        ItemDef augmentAtSlot = AugmentScreenUtilities.GetAugmentAtSlot(__instance.CurrentCharacter, keyValuePair.Key);
                        bool flag2 = augmentAtSlot != null && augmentAtSlot.Tags.Contains(____bionicsTag);
                        bool flag3 = augmentAtSlot != null && augmentAtSlot.Tags.Contains(____mutationTag);
                        if (flag2)
                        {
                            lockedReasonKey = __instance.LockedDueToBionicsKey.LocalizationKey;
                            slotState = AugumentSlotState.BlockedByPermenantAugument;
                        }
                        else if (!flag && !flag3)
                        {
                            lockedReasonKey = __instance.LockedDueToLimitKey.LocalizationKey;
                            slotState = AugumentSlotState.AugumentationLimitReached;
                        }
                        keyValuePair.Value.ResetContainer(slotState, lockedReasonKey);
                    }
                    foreach (GeoItem geoItem in __instance.CurrentCharacter.ArmourItems)
                    {
                        if (geoItem.ItemDef.Tags.Contains(____mutationTag))
                        {
                            foreach (AddonDef.RequiredSlotBind requiredSlotBind in geoItem.ItemDef.RequiredSlotBinds)
                            {
                                if (____augmentSections.ContainsKey(requiredSlotBind.RequiredSlot))
                                {
                                    ____augmentSections[requiredSlotBind.RequiredSlot].SetMutationUsed(geoItem.ItemDef);
                                }
                            }
                        }
                    }
                    string text = " " + __instance.XoutOfY.Localize(null);
                    text = text.Replace("{0}", ____currentCharacterAugmentsAmount.ToString());
                    text = text.Replace("{1}", MaxAugmentations.ToString());

                    __instance.MutationsAvailableValue.fontSize = 44;
                    __instance.MutationsAvailableValue.alignment = TextAnchor.MiddleRight;

                    __instance.MutationsAvailableValue.text = text;
                    __instance.MutationsAvailableValue.GetComponent<UIColorController>().SetWarningActive(MaxAugmentations <= ____currentCharacterAugmentsAmount, false);



                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(UIModuleBionics), "InitCharacterInfo")]
        public static class UIModuleBionics_InitCharacterInfo_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableSoldierAdjustments;
            }

            public static bool Prefix(UIModuleBionics __instance, ref int ____currentCharacterAugmentsAmount, Dictionary<AddonSlotDef, UIModuleMutationSection> ____augmentSections, GameTagDef ____bionicsTag, GameTagDef ____mutationTag)
            {
                try
                {
                    ____currentCharacterAugmentsAmount = 0;
                    ____currentCharacterAugmentsAmount = AugmentScreenUtilities.GetNumberOfAugments(__instance.CurrentCharacter);
                    bool flag = ____currentCharacterAugmentsAmount < MaxAugmentations;
                    foreach (KeyValuePair<AddonSlotDef, UIModuleMutationSection> keyValuePair in ____augmentSections)
                    {
                        AugumentSlotState slotState = AugumentSlotState.Available;
                        string lockedReasonKey = null;
                        ItemDef augmentAtSlot = AugmentScreenUtilities.GetAugmentAtSlot(__instance.CurrentCharacter, keyValuePair.Key);
                        bool flag2 = augmentAtSlot != null && augmentAtSlot.Tags.Contains(____bionicsTag);
                        if (augmentAtSlot != null && augmentAtSlot.Tags.Contains(____mutationTag))
                        {
                            lockedReasonKey = __instance.LockedDueToMutationKey.LocalizationKey;
                            slotState = AugumentSlotState.BlockedByPermenantAugument;
                        }
                        else if (!flag && !flag2)
                        {
                            lockedReasonKey = __instance.LockedDueToLimitKey.LocalizationKey;
                            slotState = AugumentSlotState.AugumentationLimitReached;
                        }
                        keyValuePair.Value.ResetContainer(slotState, lockedReasonKey);
                    }
                    foreach (GeoItem geoItem in __instance.CurrentCharacter.ArmourItems)
                    {
                        if (geoItem.ItemDef.Tags.Contains(____bionicsTag))
                        {
                            foreach (AddonDef.RequiredSlotBind requiredSlotBind in geoItem.ItemDef.RequiredSlotBinds)
                            {
                                if (____augmentSections.ContainsKey(requiredSlotBind.RequiredSlot))
                                {
                                    ____augmentSections[requiredSlotBind.RequiredSlot].SetMutationUsed(geoItem.ItemDef);
                                }
                            }
                        }
                    }
                    string text = " " + __instance.XoutOfY.Localize(null);
                    text = text.Replace("{0}", ____currentCharacterAugmentsAmount.ToString());
                    text = text.Replace("{1}", MaxAugmentations.ToString());

                    __instance.AugmentsAvailableValue.fontSize = 44;
                    __instance.AugmentsAvailableValue.alignment = TextAnchor.MiddleRight;

                    __instance.AugmentsAvailableValue.text = text;
                    __instance.AugmentsAvailableValue.GetComponent<UIColorController>().SetWarningActive(MaxAugmentations <= ____currentCharacterAugmentsAmount, false);



                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }



        [HarmonyPatch(typeof(UIModuleMutationSection), "ResetContainer")]
        public static class UIModuleMutationSection_ResetContainer_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableSoldierAdjustments;
            }

            public static void Prefix(UIModuleMutationSection __instance, AugumentSlotState slotState)
            {
                try
                {
                    if (slotState != AugumentSlotState.Available)
                    {
                        Text text = __instance.MutationLockedLoc.gameObject.GetComponent<Text>();

                        if (text != null)
                        {
                            text.fontSize = 30;
                            text.lineSpacing = 1f;
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
