using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;

namespace AssortedAdjustments.Patches
{
    internal static class DifficultyOverrides
    {
        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<GameDifficultyLevelDef> defs = defRepository.DefRepositoryDef.AllDefs.OfType<GameDifficultyLevelDef>().ToList();
            foreach (GameDifficultyLevelDef def in defs)
            {
                def.StartingSupplies = AssortedAdjustments.Settings.DifficultyOverrideStartingSupplies;
                def.StartingMaterials = AssortedAdjustments.Settings.DifficultyOverrideStartingMaterials;
                def.StartingTech = AssortedAdjustments.Settings.DifficultyOverrideStartingTech;
                def.SoldierSkillPointsPerMission = AssortedAdjustments.Settings.DifficultyOverrideSoldierSkillPointsPerMission;
                def.ExpConvertedToSkillpoints = AssortedAdjustments.Settings.DifficultyOverrideExpConvertedToSkillpoints;

                Logger.Info($"[DifficultyOverrides_Apply] def: {def.name}, GUID: {def.Guid}, StartingSupplies: {def.StartingSupplies}, StartingMaterials: {def.StartingMaterials}, StartingTech: {def.StartingTech}, SoldierSkillPointsPerMission: {def.SoldierSkillPointsPerMission}, ExpConvertedToSkillpoints: {def.ExpConvertedToSkillpoints}");
            }
        }
    }
}
