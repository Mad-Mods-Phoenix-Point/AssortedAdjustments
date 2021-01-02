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
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.View.ViewControllers.BaseRecruits;
using PhoenixPoint.Geoscape.View.DataObjects;

namespace AssortedAdjustments.Patches
{
    internal static class SoldierAdjustments
    {
        internal static int MaxAugmentations = Mathf.Clamp(AssortedAdjustments.Settings.MaxAugmentations, 0, 3);
        internal static int VanillaAbilityLimit = 3;
        internal static int ModAbilityLimit = 7;
        internal static int PersonalAbilitiesCount = Mathf.Clamp(AssortedAdjustments.Settings.PersonalAbilitiesCount, VanillaAbilityLimit, ModAbilityLimit);



        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<BaseStatSheetDef> baseStatSheetDefs = defRepository.DefRepositoryDef.AllDefs.OfType<BaseStatSheetDef>().ToList();
            foreach (BaseStatSheetDef bssDef in baseStatSheetDefs)
            {
                bssDef.PersonalAbilitiesCount = PersonalAbilitiesCount;
                bssDef.MaxStrength = AssortedAdjustments.Settings.MaxStrength;
                bssDef.MaxWill = AssortedAdjustments.Settings.MaxWill;
                bssDef.MaxSpeed = AssortedAdjustments.Settings.MaxSpeed;

                Logger.Info($"[SoldierAdjustments_Apply] bssDef: {bssDef.name}, GUID: {bssDef.Guid}, PersonalAbilitiesCount: {bssDef.PersonalAbilitiesCount}, Attributes: {bssDef.MaxStrength}, {bssDef.MaxWill}, {bssDef.MaxSpeed}");
            }
        }



        // Patches
        [HarmonyPatch(typeof(RecruitsListElementController), "SetRecruitElement")]
        public static class RecruitsListElementController_SetRecruitElement_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.EnableSoldierAdjustments;
            }

            // Prepare UI for more than 3 personal abilities
            public static void Prefix(RecruitsListElementController __instance, RecruitsListEntryData entryData, List<RowIconTextController> ____abilityIcons)
            {
                try
                {
                    RowIconTextController[] rowItems = __instance.PersonalTrackRoot.transform.GetComponentsInChildren<RowIconTextController>(true);

                    Logger.Debug($"[RecruitsListElementController_SetRecruitElement_PREFIX] rowItems: {rowItems.Length}");
                    Logger.Debug($"[RecruitsListElementController_SetRecruitElement_PREFIX] abilities: {entryData.PersonalTrackAbilities.Count()}");

                    // Only add the icon containers if we need them
                    if (rowItems.Length < ModAbilityLimit)
                    {
                        // Clone the first item as often as needed, it's filled with content in the original method by calling RecruitsListElementController.SetAbilityIcons()
                        RowIconTextController cloneBase = rowItems.FirstOrDefault();
                        int clonesNeeded = ModAbilityLimit - VanillaAbilityLimit;

                        if (cloneBase == null)
                        {
                            throw new NullReferenceException("Object to clone is null");
                        }

                        for (int i = 0; i < clonesNeeded; i++)
                        {
                            UnityEngine.Object.Instantiate<RowIconTextController>(rowItems.FirstOrDefault(), __instance.PersonalTrackRoot.transform, true);
                        }
                    }



                    rowItems = __instance.PersonalTrackRoot.transform.GetComponentsInChildren<RowIconTextController>(true);
                    Logger.Debug($"[RecruitsListElementController_SetRecruitElement_PREFIX] rowItems: {rowItems.Length}");

                    // Disable textfield and rescale item for more than 3 abilities
                    if (entryData.PersonalTrackAbilities.Count() > VanillaAbilityLimit) {
                        foreach (RowIconTextController rowItem in rowItems)
                        {
                            rowItem.DisplayText.gameObject.SetActive(false);

                            RectTransform rtRowItem = rowItem.GetComponent<RectTransform>();

                            RectTransform rtText = rowItem.DisplayText.GetComponent<RectTransform>();
                            rtText.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
                            rtRowItem.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);

                            //Rect _rect = (Rect)AccessTools.Property(typeof(RectTransform), "rect").GetValue(rtRowItem, null);
                            //_rect.width = 100f;
                        }
                    }



                    //HorizontalLayoutGroup layout = __instance.PersonalTrackRoot.GetComponent<HorizontalLayoutGroup>();
                    //Logger.Info($"{layout.name}");
                    //layout.childForceExpandWidth = false;
                    //layout.childScaleWidth = false;
                    //layout.childControlWidth = false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(RecruitsListElementController), "SetAbilityIcons")]
        public static class RecruitsListElementController_SetAbilityIcons_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.EnableSoldierAdjustments;
            }

            // Quickfix to make more than 3(three) personal abilities work (ignoring UI)
            /*
            public static void Prefix(RecruitsListElementController __instance, ref List<TacticalAbilityViewElementDef> abilities)
            {
                try
                {
                    // If the list of abilities is bigger than the vanilla limit of 3 it breaks the UI
                    // So, only grab 3 from the list to display and ignore all others (they work properly but won't be shown)

                    //abilities = abilities.Take(3).ToList();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            */

            // Add ability title to tooltips
            public static void Postfix(RecruitsListElementController __instance, List<TacticalAbilityViewElementDef> abilities, List<RowIconTextController> ____abilityIcons)
            {
                try
                {
                    Logger.Debug($"[RecruitsListElementController_SetAbilityIcons_POSTFIX] abilities.Count: {abilities.Count()}");
                    Logger.Debug($"[RecruitsListElementController_SetAbilityIcons_POSTFIX] ____abilityIcons.Count: {____abilityIcons.Count()}");

                    for (int i = 0; i < abilities.Count; i++)
                    {
                        TacticalAbilityViewElementDef tacticalAbilityViewElementDef = abilities[i];
                        ____abilityIcons[i].Tooltip.TipKey = null;
                        ____abilityIcons[i].Tooltip.TipText = $"{tacticalAbilityViewElementDef.DisplayName1.Localize()} - {tacticalAbilityViewElementDef.Description.Localize()}";
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Mutation limit
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

        // Augmentation limit
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



        // Fix UI glitches
        [HarmonyPatch(typeof(UIModuleMutationSection), "ResetContainer")]
        public static class UIModuleMutationSection_ResetContainer_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableSoldierAdjustments;
            }

            public static void Postfix(UIModuleMutationSection __instance, AugumentSlotState slotState)
            {
                try
                {
                    if (slotState != AugumentSlotState.Available)
                    {
                        Text text = __instance.MutationLockedLoc.gameObject.GetComponent<Text>();

                        if (text != null)
                        {
                            string s = text.text;
                            string[] a = s.Split((char)32);
                            string r = "\n";
                            for(int i = 0; i < a.Length; i++)
                            {
                                r += a[i];
                                r += " ";

                                if (i == 0)
                                {
                                    continue;
                                }

                                // Add a break every four words
                                if (i % 3 == 0)
                                {
                                    r += "\n";
                                }
                            }
                            text.text = r;
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
