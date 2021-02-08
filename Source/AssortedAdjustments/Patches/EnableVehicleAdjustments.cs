using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Research.Reward;
using PhoenixPoint.Tactical.Entities;

namespace AssortedAdjustments.Patches
{
    internal static class VehicleAdjustments
    {
        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<GeoVehicleDef> geoVehicleDefs = defRepository.DefRepositoryDef.AllDefs.OfType<GeoVehicleDef>().ToList();
            foreach (GeoVehicleDef gvDef in geoVehicleDefs)
            {
                if (gvDef.name.Contains("Blimp"))
                {
                    gvDef.BaseStats.Speed.Value = AssortedAdjustments.Settings.AircraftBlimpSpeed;
                    gvDef.BaseStats.SpaceForUnits = AssortedAdjustments.Settings.AircraftBlimpSpace;
                    gvDef.BaseStats.MaximumRange.Value = AssortedAdjustments.Settings.AircraftBlimpRange;
                }
                else if (gvDef.name.Contains("Thunderbird"))
                {
                    gvDef.BaseStats.Speed.Value = AssortedAdjustments.Settings.AircraftThunderbirdSpeed;
                    gvDef.BaseStats.SpaceForUnits = AssortedAdjustments.Settings.AircraftThunderbirdSpace;
                    gvDef.BaseStats.MaximumRange.Value = AssortedAdjustments.Settings.AircraftThunderbirdRange;
                }
                else if (gvDef.name.Contains("Manticore"))
                {
                    gvDef.BaseStats.Speed.Value = AssortedAdjustments.Settings.AircraftManticoreSpeed;
                    gvDef.BaseStats.SpaceForUnits = AssortedAdjustments.Settings.AircraftManticoreSpace;
                    gvDef.BaseStats.MaximumRange.Value = AssortedAdjustments.Settings.AircraftManticoreRange;
                }
                else if (gvDef.name.Contains("Helios"))
                {
                    gvDef.BaseStats.Speed.Value = AssortedAdjustments.Settings.AircraftHeliosSpeed;
                    gvDef.BaseStats.SpaceForUnits = AssortedAdjustments.Settings.AircraftHeliosSpace;
                    gvDef.BaseStats.MaximumRange.Value = AssortedAdjustments.Settings.AircraftHeliosRange;
                }

                Logger.Info($"[VehicleAdjustments_Apply] gvDef: {gvDef.name}, GUID: {gvDef.Guid}, Speed: {gvDef.BaseStats.Speed.Value}, SpaceForUnits: {gvDef.BaseStats.SpaceForUnits}, MaximumRange: {gvDef.BaseStats.MaximumRange.Value}");
            }

            foreach (TacCharacterDef tcDef in defRepository.DefRepositoryDef.AllDefs.OfType<TacCharacterDef>().Where(d => d.IsVehicle || d.IsMutog))
            {
                if (tcDef.name.Contains("CharacterTemplateDef"))
                {
                    if (tcDef.name.Contains("Mutog"))
                    {
                        tcDef.Volume = AssortedAdjustments.Settings.OccupyingSpaceMutog;
                    }
                    else if (tcDef.name.Contains("Armadillo"))
                    {
                        tcDef.Volume = AssortedAdjustments.Settings.OccupyingSpaceArmadillo;
                    }
                    else if (tcDef.name.Contains("Scarab"))
                    {
                        tcDef.Volume = AssortedAdjustments.Settings.OccupyingSpaceScarab;
                    }
                    else if (tcDef.name.Contains("Aspida"))
                    {
                        tcDef.Volume = AssortedAdjustments.Settings.OccupyingSpaceAspida;
                    }

                    Logger.Info($"[VehicleAdjustments_Apply] tcDef: {tcDef.name}, GUID: {tcDef.Guid}, Volume: {tcDef.Volume}");
                }
            }



            /*
            // The Tiamat has a research reward that adds to its range. Try to add to its speed too.
            // NOTE: It works flawlessy. Now we only need to find out how to add research items to the factions so the space/range/speed modifiers aren't a cheat but a feature... :-)
            // AND... to be implemented properly we'd need to adjust descriptions/bonus texts and manufacturing item values (vanilla does NOT even do this...)
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<AircraftBuffResearchRewardDef>())
            {
                if (def.name.Contains("AdvancedBlimp"))
                {
                    // NOTE that this is not implemented as an multiplier but as a fraction (calc in code is: base * (1f + multi))

                    // Remove range bonus
                    def.ModData.RangeMultiplier = 0;

                    // Add speed bonus
                    def.ModData.SpeedMultiplier = 0.6f; // Results in a final speed of 400 (250 * 1.6)
                }
            }
            */
        }
    }
}
