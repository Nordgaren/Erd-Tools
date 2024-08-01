using Erd_Tools.ErdToolsException;
using Erd_Tools.Utils;
using System;
using PropertyHook;
using static Erd_Tools.Models.Item;

namespace Erd_Tools.Models
{
    public class InventoryEntry
    {
        private PHPointer Pointer { get; }
        public uint Index { get; }
        public byte[] Bytes { get; }
        public string Name { get; }
        public int GaItemHandle { get; }
        public uint RawItemId { get; }
        private int _itemId;

        public int ItemID
        {
            get => _itemId;
            set
            {
                _itemId = value;
                Pointer.WriteInt32((int)Offsets.InventoryEntry.ItemID, (int)Category | value);
            }
        }

        public Category Category { get; }
        private int _quantity;

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                Pointer.WriteInt32((int)Offsets.InventoryEntry.Quantity, value);
            }
        }

        public int DisplayID { get; }

        [Obsolete("This property is deprecated and will gone, soon. Use InventoryEntry(PHPointer pointer, uint index, ErdHook hook), instead.")]
        public InventoryEntry(PHPointer pointer, uint index, byte[] bytes, ErdHook hook) : this(pointer, index, hook) { }

        public InventoryEntry(PHPointer pointer, uint index, ErdHook hook)
        {
            Pointer = pointer;
            Index = index;
            Bytes = Pointer.ReadBytes(0, Offsets.InventoryEntrySize);

            Name = "Unknown";
            GaItemHandle = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.GaItemHandle);

            RawItemId = BitConverter.ToUInt32(Bytes, (int)Offsets.InventoryEntry.ItemID);

            ItemID = (int)(RawItemId & 0x0FFFFFFF);
            Category = (Category)(RawItemId & 0xF0000000);

            Quantity = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.Quantity);

            DisplayID = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.DispalyID);

            switch (Category)
            {
                case Category.Weapons:
                    int id = Util.DeleteFromEnd(ItemID, 2) * 100;
                    int upgradeLevel = ItemID - id;
                    string levelString = upgradeLevel > 0 ? $"+{upgradeLevel}" : "";
                    if (hook.EquipParamWeapon?.NameDictionary.ContainsKey(id) ?? false)
                        Name = $"{hook.EquipParamWeapon.NameDictionary[id]}{levelString}";
                    break;
                case Category.Protector:
                    if (hook.EquipParamProtector?.NameDictionary.ContainsKey(ItemID) ?? false)
                        Name = hook.EquipParamProtector.NameDictionary[ItemID];
                    break;
                case Category.Accessory:
                    if (hook.EquipParamAccessory?.NameDictionary.ContainsKey(ItemID) ?? false)
                        Name = hook.EquipParamAccessory.NameDictionary[ItemID];
                    break;
                case Category.Goods:
                    if (hook.EquipParamGoods?.NameDictionary.ContainsKey(ItemID) ?? false)
                        Name = hook.EquipParamGoods.NameDictionary[ItemID];
                    break;
                case Category.Gem:
                    if (hook.EquipParamGem?.NameDictionary.ContainsKey(ItemID) ?? false)
                        Name = hook.EquipParamGem.NameDictionary[ItemID];
                    break;
                default:
                    throw new InvalidEntryException(
                        $"Invalid Category for Item Entry {RawItemId:X2} Category: {Category:X2}");
            }
        }
    }
}