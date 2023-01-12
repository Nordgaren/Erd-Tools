
namespace Erd_Tools
{
    public class Offsets
    {
        public const int IsLoadedOffset1 = 0x320;
        public const int IsLoadedOffset2 = 0xC8;
        public const int IsLoadedOffset3 = 0xC0;
        public const int IsLoadedOffset4 = 0x100;
        public const int IsLoaded = 0x48;

        public const int RelativePtrAddressOffset = 0x3;
        public const int RelativePtrInstructionSize = 0x7;
        public const int LargeRelativePtrInstructionSize = 0x8;

        public const string GameDataManAoB = "48 8B 05 ? ? ? ? 48 85 C0 74 05 48 8B 40 58 C3 C3";
        public const string GameManAoB = "48 8B 05 ? ? ? ? 80 B8 ? ? ? ? 0D 0F 94 C0 C3";

        public enum GameMan
        {
            LastGrace = 0xB30
        }

        public enum GameDataMan
        {
            PlayerGameData = 0x8
        }

        public enum PlayerGameData
        {
            Name = 0x9C,
            MaximumNormalItems = 0x414,
            InventoryCount = 0x420,
            MaximumSpecialItems = 0x424,
            InventoryLength = 0x488,
            HeldNormalItems = 0x450,
            HeldSpecialItems = 0x460
        }

        public enum ChrIns
        {
            ArmStyle = 0x328,
            CurrWepSlotOffsetLeft = 0x32C,
            CurrWepSlotOffsetRight = 0x330,
            LHandWeapon1 = 0x39C,
            LHandWeapon2 = 0x3A4,
            LHandWeapon3 = 0x3AC,
            RHandWeapon1 = 0x3A0,
            RHandWeapon2 = 0x3A8,
            RHandWeapon3 = 0x3B0,
            Arrow1 = 0x3B4,
            Bolt1 = 0x3B8,
            Arrow2 = 0x3BC,
            Bolt2 = 0x3C0
        }

        public enum Player
        {
            Level = 0x68
        }

        public const string SoloParamRepositoryAoB = "48 8B 0D ? ? ? ? 48 85 C9 0F 84 ? ? ? ? 45 33 C0 BA 8E 00 00 00";
        public const string CapParamCallAoB = "48 8B C4 55 41 56 41 57 48 8D 68 A1 48 81 EC A0 00 00 00 48 C7 45 E7 FE FF FF FF 48 89 58 08 48 89 70 10";

        public enum Param
        {
            ParamFileSize = -0x10,
            TotalParamLength = 0x0,
            NameOffset = 0x10,
            TableLength = 0x30,
        }

        public enum EquipParamGem
        {
            SwordArtsParamId = 0x18,
            ConfigurableWepAttr = 0x30,
            CanMountWep_Dagger = 0x38,
            CanMountWep_SwordPierce = 0x39,
            CanMountWep_SpearLarge = 0x3A,
            CanMountWep_BowSmall = 0x3B,
            CanMountWep_ShieldSmall = 0x3C,
            IsDiscard = 0x34,
            Default_WepAttr = 0x35
        }
        public enum EquipParamAccessory
        {
            IsDeposit = 0x40
        }
        public enum EquipParamGoods
        {
            MaxNum = 0x3A,
            IsFullSuppleItem = 0x4A,
            IsDrop = 0x6F
        }
        public enum EquipParamProtector
        {
            IsDiscard = 0xE3
        }
        public enum EquipParamWeapon
        {
            SortID = 0x8,
            MaterialSetID = 0x5C,
            OriginEquipWep = 0x60,
            IconID = 0xBE,
            ReinforceTypeID = 0xDA,
            BaseChangeCategory = 0x108,
            DisableMultiDropShare = 0x109,
            SwordArtsParamId = 0x198,
            WepType = 0x1A6,
            MaxArrowQuantity = 0x235,
            OriginEquipWep16 = 0x250
        }

        public const string ItemGiveAoB = "8B 02 83 F8 0A";
        //public const string ItemGiveAoB = "40 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 B0 48 81 EC 50 01 00 00 48 C7 45 C0 FE FF FF FF";
        public const string MapItemManAoB = "48 8B 0D ? ? ? ? C7 44 24 50 FF FF FF FF C7 45 A0 FF FF FF FF 48 85 C9 75 2E";
        public const int ItemGiveOffset = -0x52;

        public const int ItemInfoArraySize = 0xA0;

        public enum ItemGiveStruct
        {
            ItemStructHeaderSize = 0x24,
            ItemStructEntrySize = 0x10,
            Count = 0x20,
            ID = 0x24,
            Quantity = 0x28,
            Gem = 0x30
        }

        public const int EquipInventoryDataOffset = 0x5B8;
        public const int PlayerInventoryOffset = 0x10;
        public const int PlayInventoryEntrySize = 0x14;

        public enum InventoryEntry
        {
            GaItemHandle = 0x0,
            ItemID = 0x4,
            ItemCategory = 0x7,
            Quantity = 0x8,
            DispalyID = 0xC,
            Unk = 0x10
        }

        public const string EventFlagManAoB = "48 8B 3D ? ? ? ? 48 85 FF ? ? 32 C0 E9";
        public const string IsEventCallAoB = "48 83 EC 28 8B 12 85 D2";
        public const string SetEventCallAoB = "? ? ? ? ? 48 89 74 24 18 57 48 83 EC 30 48 8B DA 41 0F B6 F8 8B 12 48 8B F1 85 D2 0F 84 ? ? ? ? 45 84 C0";

        public const string WorldChrManAoB = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 0F 48 39 88";

        public enum WorldChrMan
        {
            NumWorldBlockChr = 0xb528,
            WorldBlockChr = 0x330,
            ChrSet1 = 0x17420,
            ChrSet2 = 0x17438
        }

        public enum WorldBlockChr
        {
            NumChr = 0x88,
            ChrSet = 0x90
        }

        public enum ChrSet
        {
            EnemyIns = 0x10,
            NumEntries = 0x20,
        }

        public enum EnemyIns
        {
            /// <summary>
            /// int EnemyHandle
            /// </summary>
            EnemyHandle = 0x8,
            /// <summary>
            /// int EnemyArea
            /// </summary>
            EnemyArea = 0xC,
            /// <summary>
            /// IntPtr EnemyCtrl
            /// </summary>
            EnemyCtrl = 0x58,
            /// <summary>
            /// int NpcParamID
            /// </summary>
            NpcParam = 0x60,
            /// <summary>
            /// int ModelNumber
            /// </summary>
            ModelNumber = 0x64,
            /// <summary>
            /// int ChrType
            /// </summary>
            ChrType = 0x68,
            /// <summary>
            /// byte TeamType
            /// </summary>
            TeamType = 0x6C,
            /// <summary>
            /// IntPtr ModuleBase
            /// </summary>
            ModuleBase = 0x190
        }

        public enum ModuleBase
        {
            /// <summary>
            /// IntPtr EnemyData
            /// </summary>
            EnemyData = 0x0,
            /// <summary>
            /// IntPtr ResistanceData
            /// </summary>
            ResistenceData = 0x20,
            /// <summary>
            /// IntPtr StaggerData
            /// </summary>
            StaggerData = 0x40,
            /// <summary>
            /// IntPtr PhysicsData
            /// </summary>
            PhysicsData = 0x68,
            /// <summary>
            /// IntPtr ChrActionRequestModule
            /// </summary>
            ActionRequest = 0x80,
        }

        public enum EnemyData
        {
            /// <summary>
            /// UnicodeString[16] Model
            /// </summary>
            Model = 0xC8,
            /// <summary>
            /// int Hp
            /// </summary>
            Hp = 0x138,
            /// <summary>
            /// int HpMax
            /// </summary>
            HpMax = 0x140,
            /// <summary>
            /// int HpBase
            /// </summary>
            HpBase = 0x144,
            /// <summary>
            /// int Fp
            /// </summary>
            Fp = 0x148,
            /// <summary>
            /// int FpMax
            /// </summary>
            FpMax = 0x14C,
            /// <summary>
            /// int FpBase
            /// </summary>
            FpBase = 0x150,
            /// <summary>
            /// int Stam
            /// </summary>
            Stam = 0x154,
            /// <summary>
            /// int StamMax
            /// </summary>
            StamMax = 0x158,
            /// <summary>
            /// int StamBase
            /// </summary>
            StamBase = 0x15C,
            /// <summary>
            /// UnicodeString[20]
            /// </summary>
            Name = 0x1A0
        }

        public enum ResistenceData
        {
            /// <summary>
            /// int Poison
            /// </summary>
            Poison = 0x10,
            /// <summary>
            /// int Rot
            /// </summary>
            Rot = 0x14,
            /// <summary>
            /// int Bleed
            /// </summary>
            Bleed = 0x18,
            /// <summary>
            /// int Blight
            /// </summary>
            Blight = 0x1C,
            /// <summary>
            /// int Frost
            /// </summary>
            Frost = 0x20,
            /// <summary>
            /// int Sleep
            /// </summary>
            Sleep = 0x24,
            /// <summary>
            /// int Madness
            /// </summary>
            Madness = 0x28,
            /// <summary>
            /// int PoisonMax
            /// </summary>
            PoisonMax = 0x2C,
            /// <summary>
            /// int RotMax
            /// </summary>
            RotMax = 0x30,
            /// <summary>
            /// int BleedMax
            /// </summary>
            BleedMax = 0x34,
            /// <summary>
            /// int BlightMax
            /// </summary>
            BlightMax = 0x38,
            /// <summary>
            /// int FrostMax
            /// </summary>
            FrostMax = 0x3C,
            /// <summary>
            /// int SleepMax
            /// </summary>
            SleepMax = 0x40,
            /// <summary>
            /// int MadnessMax
            /// </summary>
            MadnessMax = 0x44,
            /// <summary>
            /// float PoisonMod
            /// </summary>
            PoisonMod = 0x80,
            /// <summary>
            /// float RotMod
            /// </summary>
            RotMod = 0x84,
            /// <summary>
            /// float BleedMod
            /// </summary>
            BleedMod = 0x88,
            /// <summary>
            /// float BlightMod
            /// </summary>
            BlightMod = 0x8C,
            /// <summary>
            /// float FrostMod
            /// </summary>
            ForstMod = 0x90,
            /// <summary>
            /// float SleepMod
            /// </summary>
            SleepMod = 0x94,
            /// <summary>
            /// float MadnessMod
            /// </summary>
            MadnessMod = 0x98,
            /// <summary>
            /// float PoisonMul
            /// </summary>
            PoisonMul = 0x9C,
            /// <summary>
            /// float RotMul
            /// </summary>
            RotMul = 0xA0,
            /// <summary>
            /// float BleedMul
            /// </summary>
            BleedMul = 0xA4,
            /// <summary>
            /// float BlightMul
            /// </summary>
            BlightMul = 0xA8,
            /// <summary>
            /// float FrostMul
            /// </summary>
            ForstMul = 0xAC,
            /// <summary>
            /// float SleepMul
            /// </summary>
            SleepMul = 0xB0,
            /// <summary>
            /// float MadnessMul
            /// </summary>
            MadnessMul = 0xB4,
        }

        public enum StaggerData
        {
            /// <summary>
            /// float Stagger
            /// </summary>
            Stagger = 0x10,
            /// <summary>
            /// float StaggerMax
            /// </summary>
            StaggerMax = 0x14,
            /// <summary>
            /// float ResetTimer
            /// </summary>
            ResetTime = 0x1C
        }

        public enum ActionRequest
        {
            /// <summary>
            /// int CurrentAnimation
            /// </summary>
            CurrentAnimation = 0x90
        }

        public const int PlayerInsOffset = 0x1E508;

        public enum PlayerIns
        {
            TargetHandle = 0x6B0,
            TargetArea = 0x6B4
        }


        public const string DisableOpenMapAoB = "74 ?? C7 45 ?? ?? ?? ?? ?? C7 45 ?? ?? ?? ?? ?? C7 45 ?? ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 89 45 ?? 48 8D 4D ?? E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 83 BF";
        public const string CombatCloseMapAoB = "E8 ?? ?? ?? ?? 84 C0 75 ?? 38 83 ?? ?? ?? ?? 75 ?? 83 E7 FE";
        public const string WorldAreaWeatherAoB = "48 8B 15 ? ? ? ? 32 C0 48 85 D2 ? ? 8B 82";

        public enum WorldAreaWeather
        {
            ForceWeatherParamID = 0x2,
            WeatherParamID = 0x2A,
            ResetWeather = 0x88
        }

        public const string CSFD4VirtualMemoryFlagAoB = "48 8B 3D ? ? ? ? 48 85 FF 74 ? 48 8B 49";
        public const string CSLuaEventManagerAoB = "48 83 3D ? ? ? ? 00 48 8B F9 0F 84 ? ? ? ? 48";
        public const string LuaWarp_01AoB = "C3 ? ? ? ? ? ? 57 48 83 EC ? 48 8B FA 44";

        public const string GetChrInsFromHandle = "48 83 EC 28 E8 17 FF FF FF 48 85 C0 74 08 48 8B 00 48 83 C4 28 C3";
    }
}
