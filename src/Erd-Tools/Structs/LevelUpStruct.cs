using System.Runtime.InteropServices;

namespace Erd_Tools.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct LevelUpStruct
    {
        [FieldOffset(0x3C)] public int Vigor;
        [FieldOffset(0x40)] public int Mind;
        [FieldOffset(0x44)] public int Endurance;
        [FieldOffset(0x48)] public int Strength;
        [FieldOffset(0x4C)] public int Dexterity;
        [FieldOffset(0x50)] public int Intelligence;
        [FieldOffset(0x54)] public int Faith;
        [FieldOffset(0x58)] public int Arcane;
        [FieldOffset(0x68)] public int SoulLevel;
        [FieldOffset(0x64)] public int Humanity;
    }
}