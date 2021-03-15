using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Core;
using Base.Defs;
using Base.UI;
using Harmony;
using I2.Loc;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Entities.Research.Reward;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace AssortedAdjustments.Patches
{
    internal static class UnlockItemsByResearch
    {
        private static GeoPhoenixFaction PhoenixFaction => GameUtl.CurrentLevel().GetComponent<GeoLevelController>().PhoenixFaction;
        private static ItemManufacturing Manufacture => PhoenixFaction.Manufacture;
        private static SharedData SharedData;

        internal static List<string> ModifiedLocalizationTerms = new List<string>();
        private static readonly List<UnlockConfiguration> UnlockConfigurations = new List<UnlockConfiguration>();

        internal class UnlockConfiguration
        {
            internal readonly string id;

            internal List<string> UnlockableItemDefs { get; set; }
            internal List<string> RequiredResearchDefs { get; set; }

            internal string AppendCompleteText { get; set; }
            internal string AppendBenefitsText { get; set; }

            public UnlockConfiguration(string id)
            {
                this.id = id;
            }

            public int RequirementCount() => this.RequiredResearchDefs.Count;
            public bool HasSingleRequirement() => this.RequiredResearchDefs.Count == 1;
        }

        public static void Init()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            // Creating phoenix elite gear from the redeemable "gold" items by adjusting their stats, adding damage types and add manufacturing costs
            foreach (var tiDef in defRepository.DefRepositoryDef.AllDefs.OfType<TacticalItemDef>().Where(d => d.name.Contains("Gold")))
            {
                bool isStandAloneItem = tiDef.Tags.ToString().Contains("StandaloneItem_TagDef");
                if (isStandAloneItem)
                {
                    Logger.Info($"[UnlockItemsByResearch_Init] tiDef: {tiDef.name}");

                    string baseItemKey = tiDef.name.Replace("_Gold", "");
                    TacticalItemDef baseItemDef = Utilities.GetTacticalItemDef(baseItemKey);

                    tiDef.ManufactureTech = baseItemDef.ManufactureTech * 1.8f;
                    tiDef.ManufactureMaterials = baseItemDef.ManufactureMaterials * 1.5f;
                    tiDef.ManufactureMutagen = baseItemDef.ManufactureMutagen * 1.5f;
                    tiDef.ManufactureLivingCrystals = baseItemDef.ManufactureLivingCrystals * 1.5f;
                    tiDef.ManufactureOricalcum = baseItemDef.ManufactureOricalcum * 1.5f;
                    tiDef.ManufactureProteanMutane = baseItemDef.ManufactureProteanMutane * 1.5f;
                    tiDef.ManufacturePointsCost = baseItemDef.ManufacturePointsCost * 1.6f;

                    tiDef.ViewElementDef.DisplayPriority = baseItemDef.ViewElementDef.DisplayPriority + 10;

                    if (tiDef.name.Contains("Assault_Helmet"))
                    {
                        tiDef.Armor = 23;
                        tiDef.BodyPartAspectDef.Perception = 0.04f;
                        tiDef.BodyPartAspectDef.Accuracy = 0.06f;

                        string newName = "Odin-2 Helmet";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }
                    if (tiDef.name.Contains("Assault_Torso"))
                    {
                        tiDef.Armor = 24;
                        tiDef.BodyPartAspectDef.Speed = 1f;
                        tiDef.BodyPartAspectDef.Accuracy = 0.03f;

                        string newName = "Odin-2 Body Armor";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }
                    if (tiDef.name.Contains("Assault_Legs"))
                    {
                        tiDef.Armor = 22;
                        tiDef.BodyPartAspectDef.Speed = 2f;
                        tiDef.BodyPartAspectDef.Accuracy = 0.02f;

                        string newName = "Odin-2 Leg Armor";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }

                    if (tiDef.name.Contains("Heavy_Helmet"))
                    {
                        tiDef.Armor = 33;
                        tiDef.BodyPartAspectDef.Perception = -0.05f;
                        tiDef.BodyPartAspectDef.Stealth = -0.1f;
                        tiDef.BodyPartAspectDef.Accuracy = -0.04f;

                        string newName = "Golem-C Helmet";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }
                    if (tiDef.name.Contains("Heavy_Torso"))
                    {
                        tiDef.Armor = 40;
                        tiDef.BodyPartAspectDef.Stealth = -0.2f;
                        tiDef.BodyPartAspectDef.Accuracy = -0.06f;

                        string newName = "Golem-C Body Armor";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }
                    if (tiDef.name.Contains("Heavy_Legs"))
                    {
                        tiDef.Armor = 35;
                        tiDef.BodyPartAspectDef.Stealth = -0.1f;
                        tiDef.BodyPartAspectDef.Accuracy = -0.03f;

                        string newName = "Golem-C Leg Armor";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }

                    if (tiDef.name.Contains("Sniper_Helmet"))
                    {
                        tiDef.Armor = 18;
                        tiDef.BodyPartAspectDef.Perception = 7f;
                        tiDef.BodyPartAspectDef.Stealth = 0.05f;
                        tiDef.BodyPartAspectDef.Accuracy = 0.1f;

                        string newName = "Banshee-2 Helmet";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);

                    }
                    if (tiDef.name.Contains("Sniper_Torso"))
                    {
                        tiDef.Armor = 20;
                        tiDef.BodyPartAspectDef.Stealth = 0.1f;
                        tiDef.BodyPartAspectDef.Accuracy = 0.05f;

                        string newName = "Banshee-2 Body Armor";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }
                    if (tiDef.name.Contains("Sniper_Legs"))
                    {
                        tiDef.Armor = 18;
                        tiDef.BodyPartAspectDef.Stealth = 0.1f;
                        tiDef.BodyPartAspectDef.Accuracy = 0.05f;

                        string newName = "Banshee-2 Leg Armor";
                        tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                        ModifiedLocalizationTerms.Add(newName);
                    }

                    if (tiDef is WeaponDef wDef)
                    {
                        if (wDef.name.Contains("AssaultRifle"))
                        {
                            wDef.SpreadDegrees -= 0.1f;
                            wDef.DamagePayload.DamageKeywords[0].Value = 35; // Damage, default: 30
                            wDef.DamagePayload.DamageKeywords[1].Value = 2;  // Shred, default: 1

                            tiDef.ViewElementDef.DisplayPriority = baseItemDef.ViewElementDef.DisplayPriority;

                            string newName = "Ares AR-2";
                            wDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                            ModifiedLocalizationTerms.Add(newName);
                        }

                        if (wDef.name.Contains("HeavyCannon"))
                        {
                            wDef.SpreadDegrees -= 0.3f;
                            wDef.DamagePayload.DamageKeywords[0].Value = 220; // Damage, default: 200
                            wDef.DamagePayload.DamageKeywords[1].Value = 25;  // Shred, default: 20
                            wDef.DamagePayload.DamageKeywords[2].Value = 300;  // Shock, default: 280

                            tiDef.ViewElementDef.DisplayPriority = baseItemDef.ViewElementDef.DisplayPriority + 1;

                            string newName = "Hel III Cannon";
                            wDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                            ModifiedLocalizationTerms.Add(newName);
                        }

                        if (wDef.name.Contains("SniperRifle"))
                        {
                            wDef.SpreadDegrees -= 0.05f;
                            wDef.DamagePayload.DamageKeywords[0].Value = 120; // Damage, default: 110

                            DamageKeywordDef piercingDamageDef = defRepository.GetAllDefs<DamageKeywordDef>().FirstOrDefault(e => e.name == "Piercing_DamageKeywordDataDef");
                            DamageKeywordPair piercingDamage = new PhoenixPoint.Tactical.Entities.DamageKeywords.DamageKeywordPair();
                            piercingDamage.DamageKeywordDef = piercingDamageDef;
                            piercingDamage.Value = 20f;
                            wDef.DamagePayload.DamageKeywords.Add(piercingDamage);

                            tiDef.ViewElementDef.DisplayPriority = baseItemDef.ViewElementDef.DisplayPriority;

                            string newName = "Firebird SR-1";
                            wDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                            ModifiedLocalizationTerms.Add(newName);
                        }
                    }
                }
            }

            // Change the names of the related ammunition to be less specific
            foreach (var tiDef in defRepository.DefRepositoryDef.AllDefs.OfType<TacticalItemDef>().Where(d => d.name.Contains("PX") && d.name.Contains("AmmoClip")))
            {
                if (tiDef.name.Contains("AssaultRifle"))
                {
                    string newName = "Ares AR Magazine";
                    tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                    ModifiedLocalizationTerms.Add(newName);
                }

                if (tiDef.name.Contains("HeavyCannon"))
                {
                    string newName = "Hel Cannon Magazine";
                    tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                    ModifiedLocalizationTerms.Add(newName);
                }

                // Already abstract enough
                //if (tiDef.name.Contains("SniperRifle"))
                //{                 
                //      string newName = "Firebird SR Magazine";
                //      tiDef.ViewElementDef.DisplayName2 = new LocalizedTextBind(newName, true);
                //      ModifiedLocalizationTerms.Add(newName);                
                //}
            }

            // Add manufacturing costs to living weapons
            foreach (var wDef in defRepository.DefRepositoryDef.AllDefs.OfType<WeaponDef>().Where(d => d.name.Contains("AcidAssaultRifle") || d.name.Contains("PoisonMachineGun")))
            {
                wDef.ManufactureMutagen = 150f;
                wDef.ViewElementDef.DisplayPriority = 2342;

                if (wDef.name.Contains("AcidAssaultRifle"))
                {
                    wDef.ManufactureTech = 53f;
                    wDef.ManufactureMaterials = 117f;
                    wDef.ManufacturePointsCost = 170f;
                    
                }
                else if (wDef.name.Contains("PoisonMachineGun"))
                {
                    wDef.ManufactureTech = 76f;
                    wDef.ManufactureMaterials = 142f;
                    wDef.ManufacturePointsCost = 218f;
                }
            }



            // Preparing unlock configurations
            UnlockConfiguration Test = new UnlockConfiguration("Test")
            {
                UnlockableItemDefs = new List<string>()
                {
                    "PX_Heavy_Helmet_Christmas_BodyPartDef",
                    "PX_Heavy_Torso_Christmas_BodyPartDef",
                    "PX_Heavy_Legs_Christmas_ItemDef",
                    "PX_HeavyCannon_Christmas_WeaponDef",
                    "SY_Assault_Helmet_WhiteNeon_BodyPartDef",
                    "SY_Assault_Legs_WhiteNeon_ItemDef",
                    "SY_Assault_Torso_WhiteNeon_BodyPartDef",
                    "SY_LaserAssaultRifle_WhiteNeon_WeaponDef"
                },
                RequiredResearchDefs = new List<string>()
                {
                    "PX_PhoenixProject_ResearchDef"
                },
                AppendCompleteText = "",
                AppendBenefitsText = "\nNew gear is available for production"
            };

            UnlockConfiguration LivingWeapons = new UnlockConfiguration("LivingWeapons")
            {
                UnlockableItemDefs = new List<string>()
                {
                    "PX_AcidAssaultRifle_WeaponDef",
                    "PX_PoisonMachineGun_WeaponDef"
                },
                RequiredResearchDefs = new List<string>()
                {
                    "SYN_PoisonWeapons_ResearchDef",
                    "PX_AdvancedAcidTech_ResearchDef",
                    "PX_MutagenHarvesting_ResearchDef"
                },
                AppendCompleteText = "\n\nWith the combined knowledge about {0} we can now build specialized, organic weapons.",
                AppendBenefitsText = "\nNew weapons are available for production"
            };

            UnlockConfiguration PhoenixEliteGear = new UnlockConfiguration("PhoenixEliteGear")
            {
                UnlockableItemDefs = new List<string>()
                {
                    "PX_AssaultRifle_Gold_WeaponDef",
                    "PX_HeavyCannon_Gold_WeaponDef",
                    "PX_SniperRifle_Gold_WeaponDef",
                    "PX_Assault_Helmet_Gold_BodyPartDef",
                    "PX_Assault_Legs_Gold_ItemDef",
                    "PX_Assault_Torso_Gold_BodyPartDef",
                    "PX_Heavy_Helmet_Gold_BodyPartDef",
                    "PX_Heavy_Legs_Gold_ItemDef",
                    "PX_Heavy_Torso_Gold_BodyPartDef",
                    "PX_Sniper_Helmet_Gold_BodyPartDef",
                    "PX_Sniper_Legs_Gold_ItemDef",
                    "PX_Sniper_Torso_Gold_BodyPartDef"
                },
                RequiredResearchDefs = new List<string>()
                {
                    "PX_HelCannon_ResearchDef",
                    "ANU_ShreddingTech_ResearchDef",
                    "NJ_PiercerTech_ResearchDef",
                    "NJ_AutomatedFactories_ResearchDef"
                },
                AppendCompleteText = "\n\nWith the combined knowledge about {0} we can finally build higher quality versions of our basic gear.",
                AppendBenefitsText = "\nPhoenix Project Elite Gear is available for production."
            };

            UnlockConfiguration IndependentAmmunition = new UnlockConfiguration("IndependentAmmunition")
            {
                UnlockableItemDefs = new List<string>()
                {
                    "NE_AssaultRifle_AmmoClip_ItemDef",
                    "NE_MachineGun_AmmoClip_ItemDef",
                    "NE_SniperRifle_AmmoClip_ItemDef",
                    "NE_Pistol_AmmoClip_ItemDef"
                },
                RequiredResearchDefs = new List<string>()
                {
                    "PX_PhoenixProject_ResearchDef"
                },
                AppendCompleteText = "",
                AppendBenefitsText = ""
            };

            UnlockConfiguration IndependentWeapons = new UnlockConfiguration("IndependentWeapons")
            {
                UnlockableItemDefs = new List<string>()
                {
                    "NE_AssaultRifle_WeaponDef",
                    "NE_MachineGun_WeaponDef",
                    "NE_SniperRifle_WeaponDef",
                    "NE_Pistol_WeaponDef"
                },
                RequiredResearchDefs = new List<string>()
                {
                    "PX_PhoenixProject_ResearchDef"
                },
                AppendCompleteText = "",
                AppendBenefitsText = ""
            };

            UnlockConfiguration IndependentArmor = new UnlockConfiguration("IndependentArmor")
            {
                UnlockableItemDefs = new List<string>()
                {
                    "IN_Assault_Helmet_BodyPartDef",
                    "IN_Assault_Torso_BodyPartDef",
                    "IN_Assault_Legs_ItemDef",
                    "IN_Heavy_Helmet_BodyPartDef",
                    "IN_Heavy_Torso_BodyPartDef",
                    "IN_Heavy_Legs_ItemDef",
                    "IN_Sniper_Helmet_BodyPartDef",
                    "IN_Sniper_Torso_BodyPartDef",
                    "IN_Sniper_Legs_ItemDef"
                },
                RequiredResearchDefs = new List<string>()
                {
                    "PX_PhoenixProject_ResearchDef"
                },
                AppendCompleteText = "",
                AppendBenefitsText = ""
            };



            //UnlockConfigurations.Add(Test);
            if(AssortedAdjustments.Settings.UnlockPhoenixEliteGear)
            {
                UnlockConfigurations.Add(PhoenixEliteGear);
            }
            if (AssortedAdjustments.Settings.UnlockLivingWeapons)
            {
                UnlockConfigurations.Add(LivingWeapons);
            }
            if (AssortedAdjustments.Settings.UnlockIndependentAmmunition)
            {
                UnlockConfigurations.Add(IndependentAmmunition);
            }
            if (AssortedAdjustments.Settings.UnlockIndependentWeapons)
            {
                UnlockConfigurations.Add(IndependentWeapons);
            }
            if (AssortedAdjustments.Settings.UnlockIndependentArmor)
            {
                UnlockConfigurations.Add(IndependentArmor);
            }
        }



        /*
         **
         *** Utility methods
         ** 
        */

        // Add ManufacturableTag to item and add it to ItemManufacturing
        private static void UnlockItem(TacticalItemDef item)
        {
            if (SharedData == null)
            {
                SharedData = GameUtl.GameComponent<SharedData>();
            }

            if (!item.Tags.Any(e => e == SharedData.SharedGameTags.ManufacturableTag))
            {
                item.Tags.Add(SharedData.SharedGameTags.ManufacturableTag);
            }

            if (Manufacture.ManufacturableItems.Any(e => e.RelatedItemDef == item))
            {
                Logger.Info($"Already unlocked: {item.name}");
                return;
            }
            else
            {
                Manufacture.AddAvailableItem(item);
                //PhoenixFaction.NewEntityKnowledge?["item"]?.Remove(item.Guid);
            }

            if (item.CompatibleAmmunition?.Length > 0 && item.CompatibleAmmunition[0] != item)
            {
                UnlockItem(item.CompatibleAmmunition[0]);
            }
        }

        // Add custom ManufactureResearchRewardDef to a ResearchElement to unlock items
        private static void AddItemToResearchReward(List<GeoFactionDef> factions, ResearchElement researchElement, TacticalItemDef item)
        {
            if (researchElement.Rewards.Any(e => (e.BaseDef as ManufactureResearchRewardDef)?.Items?.Contains(item) ?? false))
            {
                return;
            }
            Logger.Info($"[UnlockItemsByResearch_AddItemToResearchReward] Adding item ({item.name}) to research ({researchElement.GetLocalizedName()})");

            if (SharedData == null)
            {
                SharedData = GameUtl.GameComponent<SharedData>();
            }

            if (!item.Tags.Any(e => e == SharedData.SharedGameTags.ManufacturableTag))
            {
                item.Tags.Add(SharedData.SharedGameTags.ManufacturableTag);
            }

            List<ResearchReward> rewards = researchElement.Rewards.ToList();
            List<ItemDef> items = new List<ItemDef> { item };

            if (item.CompatibleAmmunition != null)
            {
                //items.AddRange(item.CompatibleAmmunition);

                // Don't display/add ammunition if it's already unlocked
                foreach(TacticalItemDef ammo in item.CompatibleAmmunition)
                {
                    if (!ammo.Tags.Any(e => e == SharedData.SharedGameTags.ManufacturableTag))
                    {
                        ammo.Tags.Add(SharedData.SharedGameTags.ManufacturableTag);
                        items.Add(ammo);
                    }
                }
            }

            rewards.Add(new ManufactureResearchReward(new ManufactureResearchRewardDef() { Items = items.ToArray(), ValidForFactions = factions }));
            typeof(ResearchElement).GetProperty("Rewards").SetValue(researchElement, rewards.ToArray());
        }



        /*
         **
         *** Patches
         ** 
        */

        // Trigger unlocks retroactively
        [HarmonyPatch(typeof(Research), "Initialize")]
        public static class Research_Initialize_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.UnlockItemsByResearch;
            }

            public static void Postfix(Research __instance)
            {
                try
                {
                    if (!(__instance.Faction is GeoPhoenixFaction))
                    {
                        return;
                    }

                    List<GeoFactionDef> factions = new List<GeoFactionDef> { __instance.Faction.Def };
                    List<string> allCompletedResearchKeys = __instance.Completed.Select(r => r.ResearchDef.Id).ToList();

                    foreach (UnlockConfiguration c in UnlockConfigurations)
                    {
                        if (Utilities.ContainsAllItems(allCompletedResearchKeys, c.RequiredResearchDefs))
                        {
                            // If all research required for an unlock is completed, add the items to the last requirement in the list and let it be unlocked by the games method "GeoFaction.RebuildBonusesFromResearchState"
                            ResearchElement researchElement = __instance.Completed.Where(e => e.ResearchDef.Id == c.RequiredResearchDefs.Last()).FirstOrDefault();

                            if(researchElement == null)
                            {
                                throw new ArgumentException($"Couldn't fetch ResearchElement for {c.RequiredResearchDefs.Last()}");
                            }

                            foreach (string key in c.UnlockableItemDefs)
                            {
                                TacticalItemDef item = Utilities.GetTacticalItemDef(key);
                                if (item != null)
                                {
                                    //UnlockItem(item);
                                    //Logger.Debug($"Item ({key}) unlocked. All required research ({c.RequiredResearchDefs.Join()}) is completed");

                                    AddItemToResearchReward(factions, researchElement, item);
                                }
                                else
                                {
                                    Logger.Debug($"Didn't find item: {key}");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Trigger custom research rewards when a appropriate research completes
        [HarmonyPatch(typeof(ResearchElement), "Complete")]
        public static class ResearchElement_Complete_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.UnlockItemsByResearch;
            }

            public static void Prefix(ResearchElement __instance)
            {
                try
                {
                    if (!(__instance.Faction is GeoPhoenixFaction))
                    {
                        return;
                    }

                    List<GeoFactionDef> factions = new List<GeoFactionDef> { __instance.Faction.Def };
                    string currentlyCompletingResearchDef = __instance.ResearchDef.Id;
                    List<string> allCompletedResearchKeys = __instance.Faction.Research.Completed.Select(r => r.ResearchDef.Id).ToList();
                    allCompletedResearchKeys.Add(currentlyCompletingResearchDef);

                    foreach (UnlockConfiguration c in UnlockConfigurations)
                    {
                        // Currently completing research is part of the requirements AND all requirements met
                        if (c.RequiredResearchDefs.Contains(currentlyCompletingResearchDef) && Utilities.ContainsAllItems(allCompletedResearchKeys, c.RequiredResearchDefs))
                        {
                            Logger.Info($"[ResearchElement_Complete_PREFIX] All required research for {c.id} is completed: {c.RequiredResearchDefs.Join()}");

                            // Unlock items by adding it to the reward for THIS research element
                            foreach (string key in c.UnlockableItemDefs)
                            {
                                TacticalItemDef item = Utilities.GetTacticalItemDef(key);
                                if (item != null)
                                {
                                    AddItemToResearchReward(factions, __instance, item);
                                }
                                else
                                {
                                    Logger.Debug($"Couldn't find item: {key}");
                                }
                            }

                            // Visuals
                            if (!String.IsNullOrEmpty(c.AppendCompleteText))
                            {
                                int count = c.RequirementCount();
                                string requiredResearches = "";
                                for (int i = 0; i < count; i++)
                                {
                                    ResearchDef rDef = Utilities.GetResearchDef(c.RequiredResearchDefs[i]);
                                    requiredResearches += $"{rDef.ViewElementDef.DisplayName1.Localize()}";
                                    if (i == count - 2)
                                    {
                                        requiredResearches += " and ";
                                    }
                                    else if (i == count - 1)
                                    {
                                        requiredResearches += "";
                                    }
                                    else
                                    {
                                        requiredResearches += ", ";
                                    }
                                }

                                string orgCompleteText = __instance.ResearchDef.ViewElementDef.CompleteText.Localize();
                                string addCompleteText = string.Format(c.AppendCompleteText, requiredResearches);
                                __instance.ResearchDef.ViewElementDef.CompleteText = new LocalizedTextBind($"{orgCompleteText}{addCompleteText}", true);
                            }

                            if (!String.IsNullOrEmpty(c.AppendBenefitsText))
                            {
                                string orgBenefitsText = __instance.ResearchDef.ViewElementDef.BenefitsText.Localize();
                                string addBenefitsText = c.AppendBenefitsText;
                                __instance.ResearchDef.ViewElementDef.BenefitsText = new LocalizedTextBind($"{orgBenefitsText}{addBenefitsText}", true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Utility patch to support the text changes. Localization handling seems to be handled very erratic. This is a dirty "I only have a hammer so i'll use it" solution.
        [HarmonyPatch(typeof(LocalizationManager), "TryGetTranslation")]
        public static class LocalizationManager_TryGetTranslation_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.UnlockItemsByResearch;
            }

            public static void Postfix(bool __result, string Term, ref string Translation)
            {
                try
                {
                    if (__result)
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(Term) && ModifiedLocalizationTerms.Contains(Term))
                    {
                        Logger.Info($"[LocalizationManager_TryGetTranslation_POSTFIX] Found custom term: {Term}. Translation: {Translation}. Setting translation to term but keeping the return value of {__result}.");
                        Translation = Term;
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
