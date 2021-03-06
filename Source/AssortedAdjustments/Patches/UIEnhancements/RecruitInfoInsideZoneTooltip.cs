﻿using System;
using Harmony;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.View.ViewControllers.HavenDetails;
using System.Text.RegularExpressions;
using Base.UI;
using PhoenixPoint.Common.UI;
using Base.Utils;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities.Abilities;
using Base.Core;
using PhoenixPoint.Common.Core;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class RecruitInfoInsideZoneTooltip
    {
        private static bool showSkills = true;
        private static bool showAugments = true;

        internal class TooltipConfig
        {
            public string TitleHeaderTags = "<size=52><color=#FFFFFF>...</color></size>";
            public string TitleTeaserTags = "<size=36>...</size>";
            public string ItemHeaderTags = "<size=42><color=#ECBA62>...</color></size>";
            public string ItemDescTags = "<size=36>...</size>";
        }

        private static readonly TooltipConfig ConfigSkills = new TooltipConfig();
        private static readonly TooltipConfig ConfigAugmentations = new TooltipConfig { ItemHeaderTags = "<size=36><color=#B836E4>...</color></size>", ItemDescTags = "" };



        [HarmonyPatch(typeof(HavenFacilityItemController), "SetRecruitmentGroup")]
        public static class HavenFacilityItemController_SetRecruitmentGroup_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowRecruitInfoInsideZoneTooltip;
            }

            private static string BuildItem(KeyValuePair<string, string> pair, string[] tags)
            {
                return $"{tags[0]}{pair.Key}\n{tags[1]}{tags[2]}{pair.Value}{tags[3]}";
            }

            public static void Postfix(HavenFacilityItemController __instance, GeoHaven ____haven)
            {
                try
                {
                    Logger.Debug($"[HavenFacilityItemController_SetRecruitmentGroup_POSTFIX] Haven: {____haven?.name}");

                    GeoUnitDescriptor recruit = ____haven?.AvailableRecruit;
                    if (recruit == null || !recruit.UnitType.IsHuman)
                    {
                        return;
                    }
                    Logger.Debug($"[HavenFacilityItemController_SetRecruitmentGroup_POSTFIX] Recruit: {recruit.GetName()}");



                    // Override ZoneTooltip
                    string popupOverrideString = "";

                    // Custom "commands" to send to the Tooltip via HTML-comment markup
                    popupOverrideString += "<!--FONTSIZE:30-->";

                    if (showSkills)
                    {
                        PersonalAbilitiesInfo personalAbilitiesInfo = new PersonalAbilitiesInfo(recruit);
                        var personalAbilities = personalAbilitiesInfo.GetItemHeaders();

                        if (personalAbilities.Any())
                        {
                            personalAbilitiesInfo.SetTags();

                            popupOverrideString += personalAbilitiesInfo.ClassDescription().Join(e => BuildItem(e, personalAbilitiesInfo.TitleTags), "\n\n");
                            popupOverrideString += "\n\n";
                            popupOverrideString += personalAbilitiesInfo.GetItems().Join(e => BuildItem(e, personalAbilitiesInfo.ItemTags), "\n\n");
                        }
                    }

                    if (showAugments)
                    {
                        SharedData sharedData = GameUtl.GameComponent<SharedData>();
                        AugmentationInfo.AnuMutation = sharedData.SharedGameTags.AnuMutationTag;
                        AugmentationInfo.BioAugTag = sharedData.SharedGameTags.BionicalTag;
                        AugmentationInfo augmentationInfo = new AugmentationInfo(recruit);
                        var augmentations = augmentationInfo.GetItemHeaders();

                        if (augmentations.Any())
                        {
                            augmentationInfo.SetTags();

                            //popupOverrideString += "\n\n";
                            //popupOverrideString += $"<size=36><color=#FFFFFF>{new LocalizedTextBind(augmentationInfo.LabelKey).Localize()}S</color></size>";
                            popupOverrideString += "\n\n";
                            popupOverrideString += augmentationInfo.GetItems().Join(e => BuildItem(e, augmentationInfo.ItemTags), "\n\n");
                        }
                    }

                    if (!String.IsNullOrEmpty(popupOverrideString))
                    {
                        //__instance.ZoneTooltip.Enabled = true;
                        __instance.ZoneTooltip.TipText = popupOverrideString;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        internal abstract class RecruitInfo
        {
            protected readonly GeoUnitDescriptor recruit;
            protected RecruitInfo(GeoUnitDescriptor recruit)
            {
                this.recruit = recruit;
            }
            protected internal abstract TooltipConfig Config { get; }
            protected internal abstract string LabelKey { get; }

            // Markup
            internal string[] TitleTags { get; private set; }
            internal string[] ItemTags { get; private set; }

            internal void SetTags()
            {
                if (TitleTags == null)
                {
                    TitleTags = new string[] { Prefix(Config.TitleHeaderTags), Postfix(Config.TitleHeaderTags), Prefix(Config.TitleTeaserTags), Postfix(Config.TitleTeaserTags) };
                }

                if (ItemTags == null)
                {
                    ItemTags = new string[] { Prefix(Config.ItemHeaderTags), Postfix(Config.ItemHeaderTags), Prefix(Config.ItemDescTags), Postfix(Config.ItemDescTags) };
                }
            }

            private string Prefix(string txt)
            {
                if (string.IsNullOrEmpty(txt))
                {
                    return "";
                }
                int pos = txt.IndexOf("...");

                return pos <= 0 ? "" : txt.Substring(0, pos);
            }

            private string Postfix(string txt)
            {
                if (string.IsNullOrEmpty(txt))
                {
                    return "";
                }
                int pos = txt.LastIndexOf("...");

                return pos < 0 ? "" : txt.Substring(pos + 3);
            }

            // Helpers
            protected KeyValuePair<string, string> Stringify(ViewElementDef view)
            {
                string title = view.DisplayName1.Localize();
                string desc = view.Description.Localize();
                string key = view.DisplayName1.LocalizationKey;

                if (HasBraces(desc) && FindInterpolation(view) is IInterpolatableObject ipolObj)
                {
                    desc = view.GetInterpolatedDescription(ipolObj);
                }

                // Mutations
                if (view is MutationViewElementDef mutation)
                {

                    title = view.DisplayName2.Localize();
                    desc = mutation.MutationDescription.Localize();
                }

                // Class & Skills
                else if (key.IndexOf("_PERSONALTRACK_", StringComparison.Ordinal) >= 0 || key.IndexOf("KEY_CLASS_", StringComparison.Ordinal) == 0)
                {

                }

                // Armor
                else if (new Regex("^KEY_(TORSO|HEAD|LEGS)_NAME$", RegexOptions.Compiled).IsMatch(key))
                {
                    title = view.DisplayName2.Localize();
                }

                return new KeyValuePair<string, string>(title, desc);
            }

            protected bool HasBraces(string s) => s.Contains("{") && s.Contains("}");
            protected virtual IInterpolatableObject FindInterpolation(ViewElementDef view) => null;
            protected abstract IEnumerable<ViewElementDef> Views { get; }
            internal virtual IEnumerable<string> GetItemHeaders() => GetItems().Select(e => e.Key).Where(e => !string.IsNullOrEmpty(e));
            internal virtual IEnumerable<KeyValuePair<string, string>> GetItems()
            {
                if (Views == null)
                {
                    yield break;
                }
                foreach (var e in Views)
                {
                    yield return Stringify(e);
                }
            }
        }

        internal class PersonalAbilitiesInfo : RecruitInfo
        {

            internal PersonalAbilitiesInfo(GeoUnitDescriptor recruit) : base(recruit) { }
            private ViewElementDef MainClass => recruit.Progression.MainSpecDef.ViewElementDef;
            protected override IEnumerable<ViewElementDef> Views => recruit.GetPersonalAbilityTrack().AbilitiesByLevel?.Select(a => a?.Ability?.ViewElementDef).Where(e => e != null);
            protected internal override TooltipConfig Config => ConfigSkills;
            protected internal override string LabelKey => "Roster Screen/KEY_GEOROSTER_PROGRESS"; // "Training"
            internal override IEnumerable<string> GetItemHeaders() => base.GetItems().Select(e => e.Key);
            internal override IEnumerable<KeyValuePair<string, string>> GetItems() => base.GetItems();
            internal IEnumerable<KeyValuePair<string, string>> ClassDescription()
            {
                yield return new KeyValuePair<string, string>(MainClass.DisplayName1.Localize(), MainClass.DisplayName2.Localize());
            }
        }

        internal abstract class ItemInfo : RecruitInfo
        {
            internal ItemInfo(GeoUnitDescriptor recruit) : base(recruit) { }
            protected abstract IEnumerable<TacticalItemDef> ItemDefs { get; }
            protected override IEnumerable<ViewElementDef> Views => ItemDefs.Select(e => e.ViewElementDef).Where(e => e != null);
            protected override IInterpolatableObject FindInterpolation(ViewElementDef view) => ItemDefs.First(e => e.ViewElementDef == view)?.Abilities?.OfType<TacticalAbilityDef>().First();
        }

        internal class AugmentationInfo : ItemInfo
        {
            internal static GameTagDef AnuMutation, BioAugTag;
            internal AugmentationInfo(GeoUnitDescriptor recruit) : base(recruit) { }
            protected internal override TooltipConfig Config => ConfigAugmentations;
            protected internal override string LabelKey => "Blood Titanium/KEY_AUGMENT"; // "Augmentation"
            protected override IEnumerable<TacticalItemDef> ItemDefs => recruit.ArmorItems.Where(IsAugmentation);
            internal static bool IsAugmentation(ItemDef def) => def.Tags.Any(tag => tag == AnuMutation || tag == BioAugTag);
        }
    }
}
