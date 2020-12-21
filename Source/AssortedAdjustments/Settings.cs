namespace AssortedAdjustments
{
    internal class Settings
    {
        public bool EnableExperienceToSkillpointConversion = true;
        public float XPtoSPConversionRate = 0.01f; // Default is dependent on difficulty setting, this is just a fallback if the setting is unretrievable.
        public float XPtoSPConversionMultiplier = 2f; // Flat multiplier for the setting above



        public bool EnableReturnFireAdjustments = true;
        public int ReturnFireAngle = 225; // Default: 360 (as in not implemented)
        public int ReturnFireLimit = 2; // Default: Unlimited



        public bool EnableMissionAdjustments = true;
        public int MaxPlayerUnitsAdd = 2; // Adding one to the default of 8 (=> 9, 21)



        public bool EnablePlentifulItemDrops = true;
        public int ItemDestructionChance = 10; // For most items this is currently set to 80 in vanilla
        public bool OverrideWeaponDrops = true;
        public int FlatWeaponDestructionChance = 30;
        public bool HealthBasedWeaponDestruction = true;



        public bool SkipIntroLogos = true;
        public bool SkipIntroMovie = true;



        public bool EnableFacilityAdjustments = true;

        // Healing
        public float MedicalBayBaseHeal = 8f; // Default: 4f
        public int VehicleBayVehicleHealAmount = 40; // Default: 20

        // Resource Generators
        public float FabricationPlantGenerateProductionAmount = 4f; // Default: 4f
        public float ResearchLabGenerateResearchAmount = 4f; // Default: 4f;
        public float FoodProductionGenerateSuppliesAmount = 0.5f; // Default: 0.33f (Translates to 8 food units, base value seems to be 24)
        public float BionicsLabGenerateResearchAmount = 4f; // Default: 4f
        public float MutationLabGenerateMutagenAmount = 0.25f; // Default: 0.25f

        // Add Tech & Materials to Facilities
        public float FabricationPlantGenerateMaterialsAmount = 1f;
        public float ResearchLabGenerateTechAmount = 1f;
        internal float GenerateResourcesBaseDivisor = 24f;



        public bool EnableSoldierAdjustments = false;
        public int PersonalAbilitiesCount = 5; // Default: 3
        public int MaxStrength = 35; // Default: 30
        public int MaxWill = 25; // Default: 20
        public int MaxSpeed = 20; // Default: 20



        public bool EnableVehicleAdjustments = true;
        public float AircraftBlimpSpeed = 300f; // Default: 250
        public float AircraftThunderbirdSpeed = 400f; // Default: 380
        public float AircraftManticoreSpeed = 550f; // Default: 500
        public float AircraftHeliosSpeed = 700f; // Default: 650
        public int AircraftBlimpSpace = 9; // Default: 8
        public int AircraftThunderbirdSpace = 8; // Default: 7
        public int AircraftManticoreSpace = 7; // Default: 6
        public int AircraftHeliosSpace = 6; // Default: 5



        public bool EnableEconomyAdjustments = true;
        public float ResourceMultiplier = 0.75f; // Resource costs
        public float ScrapMultiplier = 0.75f; // Default: 0.5f (Same as vanilla's divisor of 2f)
        public float CostMultiplier = 0.5f; // Manufacturing time



        public bool DisableAmbushes = true;
        internal bool RetainAmbushesInsideMist = true;



        public bool DisableRightClickMove = true;



        public bool DisableRocksAtBases = true;



        public bool PauseOnDestinationSet = true;
        public bool PauseOnHealed = true;
        internal bool CenterOnHealed = true;
    }
}
