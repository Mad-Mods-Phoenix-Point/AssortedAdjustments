using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Levels.Missions;

namespace AssortedAdjustments.Patches
{
    internal static class MissionAdjustments
    {
        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<TacMissionTypeDef> defs = defRepository.DefRepositoryDef.AllDefs.OfType<TacMissionTypeDef>().ToList();
            foreach (TacMissionTypeDef def in defs)
            {
                def.MaxPlayerUnits += AssortedAdjustments.Settings.MaxPlayerUnitsAdd;

                //Logger.Info($"[MissionAdjustments_Apply] def: {def.name}, GUID: {def.Guid}, MaxPlayerUnits: {def.MaxPlayerUnits}");
            }
        }
    }
}
