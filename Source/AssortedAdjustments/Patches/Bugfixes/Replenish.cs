using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssortedAdjustments.Patches.Bugfixes
{
    // Verifiying potential loadout changes (additionally checking the order of items)
    [HarmonyPatch]
    public static class PostmissionReplenishManager___Loadout_FindDifferences_Patch
    {
        public static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("Loadout");
            return AccessTools.Method(type, "FindDifferences");
        }

        public static void Postfix(ref bool __result, IReadOnlyList<GeoItem> oldList, IReadOnlyList<GeoItem> newList, GeoCharacter character, string listName)
        {
            try
            {
                // If original methods returns true the lists are already of different length
                if (__result)
                {
                    return;
                }
                Logger.Info($"[PostmissionReplenishManager.Loadout_FindDifferences_PREFIX] Verifying potential changes to {character.DisplayName}'s {listName}.");

                // Check thoroughly
                for (int i = 0; i < oldList.Count; i++)
                {
                    //Logger.Info($"[PostmissionReplenishManager.Loadout_FindDifferences_PREFIX] oldList[{i}]: {oldList[i].ItemDef.name}, newList[{i}]: {newList[i].ItemDef.name}");
                    if (oldList[i] != newList[i])
                    {
                        Logger.Info($"[PostmissionReplenishManager.Loadout_FindDifferences_PREFIX] Items at index {i} don't match (oldList[{i}]: {oldList[i].ItemDef?.ViewElementDef?.DisplayName1.Localize()}, newList[{i}]: {newList[i].ItemDef?.ViewElementDef?.DisplayName1.Localize()}).");
                        Logger.Info($"[PostmissionReplenishManager.Loadout_FindDifferences_PREFIX] Overriding result to true (game will update the preferred loadout of this character).");
                        __result = true;
                        return;
                    }
                }
                //Logger.Info($"[PostmissionReplenishManager.Loadout_FindDifferences_PREFIX] {character.DisplayName}'s {listName} matches their last stored preferred loadout.");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }



    // Bugfix for the replenisher which tries to replenish dismissed characters
    [HarmonyPatch(typeof(GeoscapeView), "QueueReplenishState")]
    public static class GeoscapeView_QueueReplenishState_Patch
    {
        public static bool Prefix(GeoscapeView __instance, GeoscapeViewContext ____context)
        {
            try
            {
                Logger.Info($"[UIModuleReplenish_Init_PREFIX] Checking for obsolete characters in PostmissionReplenishManager.Loadouts");

                GeoPhoenixFaction geoPhoenixFaction = ____context.ViewerFaction as GeoPhoenixFaction;
                if (geoPhoenixFaction != null)
                {
                    List<PostmissionReplenishManager.ReplenishableItems> missingItems = geoPhoenixFaction.GetMissingItems();

                    if (missingItems.Any<PostmissionReplenishManager.ReplenishableItems>())
                    {
                        List<GeoCharacter> allCharacters = geoPhoenixFaction.Soldiers.ToList();
                        List<GeoCharacter> obsoleteCharacters = new List<GeoCharacter>();

                        // Checking characters from missing items to find possible leftovers from the lazily implemented dismissal call
                        // Soldiers that get dismissed while having an incomplete loadout stay in the loadout list of PostmissionReplenishManager forever...
                        foreach (PostmissionReplenishManager.ReplenishableItems replenishableItems in missingItems)
                        {
                            Logger.Info($"[GeoscapeView_QueueReplenishState_PREFIX] {replenishableItems.Character.DisplayName} has missing items...");

                            if (!allCharacters.Contains(replenishableItems.Character))
                            {
                                Logger.Info($"[GeoscapeView_QueueReplenishState_PREFIX] {replenishableItems.Character.DisplayName} doesn't exit anymore...");

                                obsoleteCharacters.Add(replenishableItems.Character);
                            }
                        }

                        if (obsoleteCharacters.Count > 0)
                        {
                            PostmissionReplenishManager ____replenisher = (PostmissionReplenishManager)AccessTools.Field(typeof(GeoPhoenixFaction), "_replenisher").GetValue(geoPhoenixFaction);
                            foreach (GeoCharacter c in obsoleteCharacters)
                            {
                                Logger.Info($"[UIModuleReplenish_Init_PREFIX] {c.DisplayName} will be removed from PostmissionReplenishManager!");

                                ____replenisher.RemoveLoadout(c);
                            }
                            missingItems.RemoveAll(i => obsoleteCharacters.Contains(i.Character));
                        }

                        // If the list is empty after this cleanup, don't even switch to the replenish view state
                        if (missingItems.Count <= 0)
                        {
                            return false;
                        }
                    }
                }



                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return true;
            }
        }
    }
}
