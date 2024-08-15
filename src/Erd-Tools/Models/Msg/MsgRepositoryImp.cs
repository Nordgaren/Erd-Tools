using PropertyHook;
using SoulsFormats;
using System;

namespace Erd_Tools.Models.Msg
{
    /// <summary>
    /// MsgRepositoryImp class that can be used to get strings from the game. This class caches FMGs that have already
    /// been read, for faster access.  
    /// </summary>
    public class MsgRepositoryImp
    {
        private PHPointer _msgRepositoryImp { get; }
        private ErdHook _hook { get; }
        private FMG?[]? _cache { get; set;  }

        /// <summary>
        /// MsgRepositoryImp class responsible for interacting with the games `MsgRepositoryImp`.  
        /// </summary>
        /// <param name="msgRepositoryImpPtr"></param>
        /// <param name="hook"></param>
        public MsgRepositoryImp(PHPointer msgRepositoryImpPtr, ErdHook hook)
        {
            _msgRepositoryImp = msgRepositoryImpPtr;
            _hook = hook;
        }

        /// <summary>
        /// Gets the FMG entry from the specified FMG. FMGs are cached. Call `ClearCache()` if you need to re-read the
        /// FMGs.  
        /// </summary>
        /// <param name="fmgId">The FMG for the string lookup.</param>
        /// <param name="id">Id of the string entry</param>
        /// <returns>string entry if it exists, otherwise null</returns>
        public string? GetEntry(FmgId fmgId, int id)
        {
            FMG? cached = GetCachedFmg(fmgId);
            if (cached != null)
            {
                return cached[id];
            }
            
            PHPointer versionArray = _hook.CreateChildPointer(_msgRepositoryImp, (int)Offsets.MsgRepositoryImp.Categories);
            PHPointer categoryArray = _hook.CreateChildPointer(versionArray, 0x0);
            PHPointer fmgEntry = _hook.CreateChildPointer(categoryArray, (int)fmgId * 8);
            if (fmgEntry.Resolve() == IntPtr.Zero)
            {
                return null;
            }

            uint size = fmgEntry.ReadUInt32((int)Offsets.FmgFile.FileSize);
            byte[] bytes = fmgEntry.ReadBytes(0x0, size);
            ulong ptr = fmgEntry.ReadUInt64((int)Offsets.FmgFile.StringOffsetsOffset);
            ulong offset = ptr - (ulong)fmgEntry.Resolve();
            Array.Copy(BitConverter.GetBytes(offset), 0x0, bytes, (int)Offsets.FmgFile.StringOffsetsOffset, 0x8);
            FMG fmg = FMG.Read(bytes);

            // I know it's not null, cause GetCachedFmg will initialize it.
            _cache![(int)fmgId] = fmg;

            return fmg[id];
        }

        private FMG? GetCachedFmg(FmgId fmgId) {
            if (_cache != null)
            {
                return _cache[(int)fmgId];
            }

            ClearCache();
            return _cache[(int)fmgId];
        }

        /// <summary>
        /// Clears the fmg cache, so that FMGs will be re-read from memory.  
        /// </summary>
        public void ClearCache()
        {
            int count = _msgRepositoryImp.ReadInt32((int)Offsets.MsgRepositoryImp.CategoryCount);
            FMG[] cache = Array.Empty<FMG>();
            Array.Resize(ref cache, count);
            _cache = cache;
        }
    }

    /// <summary>
    /// Enum that represents the Fmg by name. The value is the index into the array that the MsgRepositoryImp stores the
    /// files in.  
    /// </summary>
    public enum FmgId
    {
        // Item FMGs
        GoodsName = 0x0A,
        WeaponName = 0x0B,
        ProtectorName = 0x0C,
        AccessoryName = 0x0D,
        MagicName = 0x0E,
        NpcName = 0x12,
        PlaceName = 0x13,
        GoodsInfo = 0x14,
        WeaponInfo = 0x15,
        ProtectorInfo = 0x16,
        AccessoryInfo = 0x17,
        GoodsCaption = 0x18,
        WeaponCaption = 0x19,
        ProtectorCaption = 0x1A,
        AccessoryCaption = 0x1B,
        MagicInfo = 0x1C,
        MagicCaption = 0x1D,
        GemName = 0x23,
        GemInfo = 0x24,
        GemCaption = 0x25,
        GoodsDialog = 0x29,
        ArtsName = 0x2A,
        ArtsCaption = 0x2B,
        WeaponEffect = 0x2C,
        GemEffect = 0x2D,
        GoodsInfo2 = 0x2E,
        WeaponName_dlc01 = 0x136,
        WeaponInfo_dlc01 = 0x137,
        WeaponCaption_dlc01 = 0x138,
        ProtectorName_dlc01 = 0x139,
        ProtectorInfo_dlc01 = 0x13A,
        ProtectorCaption_dlc01 = 0x13B,
        AccessoryName_dlc01 = 0x13C,
        AccessoryInfo_dlc01 = 0x13D,
        AccessoryCaption_dlc01 = 0x13E,
        GoodsName_dlc01 = 0x13F,
        GoodsInfo_dlc01 = 0x140,
        GoodsCaption_dlc01 = 0x141,
        GemName_dlc01 = 0x142,
        GemInfo_dlc01 = 0x143,
        GemCaption_dlc01 = 0x144,
        MagicName_dlc01 = 0x145,
        MagicInfo_dlc01 = 0x146,
        MagicCaption_dlc01 = 0x147,
        NpcName_dlc01 = 0x148,
        PlaceName_dlc01 = 0x149,
        GoodsDialog_dlc01 = 0x14A,
        ArtsName_dlc01 = 0x14B,
        ArtsCaption_dlc01 = 0x14C,
        WeaponEffect_dlc01 = 0x14D,
        GemEffect_dlc01 = 0x14E,
        GoodsInfo2_dlc01 = 0x14F,
        WeaponName_dlc02 = 0x19A,
        WeaponInfo_dlc02 = 0x19B,
        WeaponCaption_dlc02 = 0x19C,
        ProtectorName_dlc02 = 0x19D,
        ProtectorInfo_dlc02 = 0x19E,
        ProtectorCaption_dlc02 = 0x19F,
        AccessoryName_dlc02 = 0x1A0,
        AccessoryInfo_dlc02 = 0x1A1,
        AccessoryCaption_dlc02 = 0x1A2,
        GoodsName_dlc02 = 0x1A3,
        GoodsInfo_dlc02 = 0x1A4,
        GoodsCaption_dlc02 = 0x1A5,
        GemName_dlc02 = 0x1A6,
        GemInfo_dlc02 = 0x1A7,
        GemCaption_dlc02 = 0x1A8,
        MagicName_dlc02 = 0x1A9,
        MagicInfo_dlc02 = 0x1AA,
        MagicCaption_dlc02 = 0x1AB,
        NpcName_dlc02 = 0x1AC,
        PlaceName_dlc02 = 0x1AD,
        GoodsDialog_dlc02 = 0x1AE,
        ArtsName_dlc02 = 0x1AF,
        ArtsCaption_dlc02 = 0x1B0,
        WeaponEffect_dlc02 = 0x1B1,
        GemEffect_dlc02 = 0x1B2,
        GoodsInfo2_dlc02 = 0x1B3,
        TalkMsg = 0x01,
        BloodMsg = 0x02,
        MovieSubtitle = 0x03,
        NetworkMessage = 0x1F,
        ActionButtonText = 0x20,
        EventTextForTalk = 0x21,
        EventTextForMap = 0x22,
        // Menu FMGs
        GR_MenuText = 0xC8,
        GR_LineHelp = 0xC9,
        GR_KeyGuide = 0xCA,
        GR_System_Message_win64 = 0xCB,
        GR_Dialogues = 0xCC,
        LoadingTitle = 0xCD,
        LoadingText = 0xCE,
        TutorialTitle = 0xCF,
        TutorialBody = 0xD0,
        TextEmbedImageName_win64 = 0xD1,
        ToS_win64 = 0xD2,
        TalkMsg_dlc01 = 0x168,
        BloodMsg_dlc01 = 0x169,
        MovieSubtitle_dlc01 = 0x16A,
        NetworkMessage_dlc01 = 0x16C,
        ActionButtonText_dlc01 = 0x16D,
        EventTextForTalk_dlc01 = 0x16E,
        EventTextForMap_dlc01 = 0x16F,
        GR_MenuText_dlc01 = 0x170,
        GR_LineHelp_dlc01 = 0x171,
        GR_KeyGuide_dlc01 = 0x172,
        GR_System_Message_win64_dlc01 = 0x173,
        GR_Dialogues_dlc01 = 0x174,
        LoadingTitle_dlc01 = 0x175,
        LoadingText_dlc01 = 0x176,
        TutorialTitle_dlc01 = 0x177,
        TutorialBody_dlc01 = 0x178,
        TalkMsg_dlc02 = 0x1CC,
        BloodMsg_dlc02 = 0x1CD,
        MovieSubtitle_dlc02 = 0x1CE,
        NetworkMessage_dlc02 = 0x1D0,
        ActionButtonText_dlc02 = 0x1D1,
        EventTextForTalk_dlc02 = 0x1D2,
        EventTextForMap_dlc02 = 0x1D3,
        GR_MenuText_dlc02 = 0x1D4,
        GR_LineHelp_dlc02 = 0x1D5,
        GR_KeyGuide_dlc02 = 0x1D6,
        GR_System_Message_win64_dlc02 = 0x1D7,
        GR_Dialogues_dlc02 = 0x1D8,
        LoadingTitle_dlc02 = 0x1D9,
        LoadingText_dlc02 = 0x1DA,
        TutorialTitle_dlc02 = 0x1DB,
        TutorialBody_dlc02 = 0x1DC,
    }
}