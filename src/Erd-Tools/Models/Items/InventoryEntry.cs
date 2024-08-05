using Erd_Tools.ErdToolsException;
using Erd_Tools.Models.Msg;
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
        
        public bool IsNew { get; }
        
        public PotType PotType { get; }

        [Obsolete(
            "This property is deprecated and will be gone, soon. Use InventoryEntry(PHPointer pointer, uint index, ErdHook hook), instead.")]
        public InventoryEntry(PHPointer pointer, uint index, byte[] bytes, ErdHook hook) : this(pointer, index, hook)
        {
        }

        public InventoryEntry(PHPointer pointer, uint index, ErdHook hook)
        {
            Pointer = pointer;
            Index = index;
            Bytes = Pointer.ReadBytes(0, Offsets.InventoryEntrySize);

            Name = "Unknown";
            GaItemHandle = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.GaItemHandle);

            RawItemId = BitConverter.ToUInt32(Bytes, (int)Offsets.InventoryEntry.ItemID);

            _itemId = (int)(RawItemId & 0x0FFFFFFF);
            Category = (Category)(RawItemId & 0xF0000000);

            _quantity = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.Quantity);

            DisplayID = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.DisplayID);
            
            IsNew = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.IsNew) != 0;
            
            PotType = (PotType)BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.PotType);

            switch (Category)
            {
                case Category.Weapons:
                    int id = Util.DeleteFromEnd(ItemID, 2) * 100;
                    int upgradeLevel = ItemID - id;
                    string levelString = upgradeLevel > 0 ? $"+{upgradeLevel}" : "";
                    string? wep = (hook.MsgRepository.GetEntry(FmgId.WeaponName, id) ??
                                   hook.MsgRepository.GetEntry(FmgId.WeaponName_dlc01, id)) ??
                                  hook.MsgRepository.GetEntry(FmgId.WeaponName_dlc02, id);

                    if (wep != null)
                    {
                        Name = $"{wep} {levelString}";
                    }
                    
                    break;
                case Category.Protector:
                    string? prot = (hook.MsgRepository.GetEntry(FmgId.ProtectorName, ItemID) ??
                                    hook.MsgRepository.GetEntry(FmgId.ProtectorName_dlc01, ItemID)) ??
                                   hook.MsgRepository.GetEntry(FmgId.ProtectorName_dlc02, ItemID);

                    if (prot != null)
                    {
                        Name = prot;
                    }
                    
                    break;
                case Category.Accessory:
                    string? acc = (hook.MsgRepository.GetEntry(FmgId.AccessoryName, ItemID) ??
                                   hook.MsgRepository.GetEntry(FmgId.AccessoryName_dlc01, ItemID)) ??
                                  hook.MsgRepository.GetEntry(FmgId.AccessoryName_dlc02, ItemID);

                    if (acc != null)
                    {
                        Name = hook.EquipParamAccessory.NameDictionary[ItemID];
                    }
                    break;
                case Category.Goods:
                    string? good = (hook.MsgRepository.GetEntry(FmgId.GoodsName, ItemID) ??
                                   hook.MsgRepository.GetEntry(FmgId.GoodsName_dlc01, ItemID)) ??
                                  hook.MsgRepository.GetEntry(FmgId.GoodsName_dlc02, ItemID);

                    if (good != null)
                    {
                        Name = good;
                    }
                    break;
                case Category.Gem:
                    string? gem = (hook.MsgRepository.GetEntry(FmgId.GemName, ItemID) ??
                                    hook.MsgRepository.GetEntry(FmgId.GemName_dlc01, ItemID)) ??
                                   hook.MsgRepository.GetEntry(FmgId.GemName_dlc02, ItemID);

                    if (gem != null)
                    {
                        Name = gem;
                    }
                    break;
                default:
                    throw new InvalidEntryException(
                        $"Invalid Category for Item Entry {RawItemId:X2} Category: {Category:X2}");
            }
        }
    }
}