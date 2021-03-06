using System.Collections.Generic;

namespace AssortedAdjustments.LimitedWar
{
    internal static class Config
    {
        public static bool Enable = true;
        public static bool LimitFactionAttacksToZones = true;
        public static bool LimitPandoranAttacksToZones = false;
        public static bool AttacksRaiseHavenAlertness = true;
        public static bool AttacksRaiseFactionAlertness = true;

        public static bool StopOneSidedWar = true;
        public static bool DisablePandoranAttacksOnPhoenixBases = false;
        public static KeyValuePair<bool, int> StopAttacksWhileDefendingPandoransThreshold = new KeyValuePair<bool, int>(true, 1);
        public static KeyValuePair<bool, int> GlobalAttackLimit = new KeyValuePair<bool, int>(true, 3);
        public static KeyValuePair<bool, int> FactionAttackLimit = new KeyValuePair<bool, int>(true, 2);
        public static bool UseDifficultyDrivenLimits = false;

        public static DefenseMultipliers DefenseMultipliers = new DefenseMultipliers();
        internal static bool HasAttackLimitsActive => StopOneSidedWar || StopAttacksWhileDefendingPandoransThreshold.Key || GlobalAttackLimit.Key || FactionAttackLimit.Key;
        
        public static void MergeSettings(Settings settings)
        {
            Enable = settings.EnableLimitedWar;
            LimitFactionAttacksToZones = settings.LimitFactionAttacksToZones;
            LimitPandoranAttacksToZones = settings.LimitPandoranAttacksToZones;
            AttacksRaiseHavenAlertness = settings.AttacksRaiseHavenAlertness;
            AttacksRaiseFactionAlertness = settings.AttacksRaiseFactionAlertness;

            StopOneSidedWar = settings.StopOneSidedWar;
            DisablePandoranAttacksOnPhoenixBases = settings.DisablePandoranAttacksOnPhoenixBases;

            if(settings.StopAttacksWhileDefendingPandoransThreshold < 0)
            {
                StopAttacksWhileDefendingPandoransThreshold = new KeyValuePair<bool, int>(false, int.MaxValue);
            }
            else
            {
                StopAttacksWhileDefendingPandoransThreshold = new KeyValuePair<bool, int>(true, settings.StopAttacksWhileDefendingPandoransThreshold);
            }

            if (settings.GlobalAttackLimit < 0)
            {
                GlobalAttackLimit = new KeyValuePair<bool, int>(false, int.MaxValue);
            }
            else
            {
                GlobalAttackLimit = new KeyValuePair<bool, int>(true, settings.GlobalAttackLimit);
            }

            if (settings.FactionAttackLimit < 0)
            {
                FactionAttackLimit = new KeyValuePair<bool, int>(false, int.MaxValue);
            }
            else
            {
                FactionAttackLimit = new KeyValuePair<bool, int>(true, settings.FactionAttackLimit);
            }

            UseDifficultyDrivenLimits = settings.UseDifficultyDrivenLimits;
        }
    }

    internal class DefenseMultipliers
    {
        public float Default = 1;
        public float Alert = 1.2f;
        public float HighAlert = 1.1f;
        public float AttackerPandora = 1.2f;
        public float AttackerAnu = 1;
        public float AttackerNewJericho = 1f;
        public float AttackerSynedrion = 1;
        public float DefenderPandora = 1;
        public float DefenderAnu = 1.2f;
        public float DefenderNewJericho = 1;
        public float DefenderSynedrion = 1.2f;

        internal bool Enabled => Default != 1 || Alert != 1 || HighAlert != 1 || AttackerPandora != 1 || AttackerAnu != 1 || AttackerNewJericho != 1 || AttackerSynedrion != 1 || DefenderPandora != 1 || DefenderAnu != 1 || DefenderNewJericho != 1 || DefenderSynedrion != 1;
    }
}