using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Erd_Tools.Models
{
    public class Item
    {
        public enum Category : uint
        {
            Weapons = 0x00000000,
            Protector = 0x10000000,
            Accessory = 0x20000000,
            Goods = 0x40000000,
            Gem = 0x80000000
        }

        public enum PotType
        {
            [Description("Cracked Pot")]
            CrackedPot = 1,
            [Description("Perfume Bottle")]
            PerfumeBottle = 2,
            [Description("Ritual Pot")]
            RitualPot = 3,
            [Description("Hefty Cracked Pot")]
            HeftyCrackedPot = 4,
            None = -1,
        }
        private static Regex ItemEntryRx = new(@"^\s*(?<id>\S+)\s+(?<name>.*)$", RegexOptions.CultureInvariant);

        public string Name;
        public int ID;
        public Category ItemCategory;
        public bool ShowID;

        public short MaxQuantity;
        public int EventID;
        public bool IsDrop;
        public bool IsMultiplayerShare;
        public bool CanAquireFromOtherPlayers => IsDrop && IsMultiplayerShare;

        public short IconID;

        public Item(Item source) {
            Name = source.Name;
            ID = source.ID;
            ItemCategory = source.ItemCategory;
            ShowID = source.ShowID;
            MaxQuantity = source.MaxQuantity;
            EventID = source.EventID;
            IsDrop = source.IsDrop;
            IsMultiplayerShare = source.IsMultiplayerShare;
        }
        public Item(string config, Category category, bool showID)
        {
            Match itemEntry = ItemEntryRx.Match(config);
            Name = itemEntry.Groups["name"].Value.Replace("\r", "");
            ID = Convert.ToInt32(itemEntry.Groups["id"].Value);
            ShowID = showID;
            ItemCategory = category;
            MaxQuantity = 1;
        }

        public override string ToString()
        {
            if (ShowID)
                return $"{ID} {Name}";
            else
                return Name;
        }

        public virtual void SetupItem(Param? param)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));

            if (!param.OffsetDict.ContainsKey(ID))
                return;
            //throw new Exception($"{Name} does not have a param entry in this version of Elden Ring!");

            byte bitfield;
            switch (ItemCategory)
            {
                case Category.Protector:
                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)Offsets.EquipParamProtector.IsDiscard];
                    IsDrop = (bitfield & (1 << 1)) != 0;
                    IsMultiplayerShare = (bitfield & (1 << 2)) == 0;
                    IconID = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamProtector.IconID);
                    break;
                case Category.Accessory:
                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)Offsets.EquipParamAccessory.IsDeposit];
                    IsMultiplayerShare = (bitfield & (1 << 2)) == 0;
                    IsDrop = (bitfield & (1 << 4)) != 0;
                    IconID = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamAccessory.IconID);
                    break;
                case Category.Goods:
                    MaxQuantity = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamGoods.MaxNum);

                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)Offsets.EquipParamGoods.IsFullSuppleItem];
                    IsMultiplayerShare = (bitfield & (1 << 3)) == 0;

                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)Offsets.EquipParamGoods.IsDrop];
                    IsDrop = (bitfield & (1 << 0)) != 0;
                    IconID = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamGoods.IconID);
                    break;
                case Category.Gem:
                case Category.Weapons:
                    break;
                default:
                    throw new("Item Does not have a proper category.");
            }
        }

    }
}
