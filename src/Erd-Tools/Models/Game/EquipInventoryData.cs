using PropertyHook;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Erd_Tools.Models.Game
{
    /// <summary>
    /// EquipInventoryData class for handling the normal and key item inventories, and related functions. 
    /// </summary>
    public class EquipInventoryData
    {
        private ErdHook _hook;
        private PHPointer _equipInventoryData;
        private PHPointer _normalItems;
        private PHPointer _keyItems;

        public EquipInventoryData(PHPointer equipInventoryData, ErdHook hook)
        {
            _equipInventoryData = equipInventoryData;
            _hook = hook;
            NormalItems =
                new InventoryItemList(equipInventoryData, (int)Offsets.EquipInventoryData.NormalInventory, _hook);
            KeyItems = new InventoryItemList(equipInventoryData, (int)Offsets.EquipInventoryData.KeyInventory, _hook);
            UnkItems = new InventoryItemList(equipInventoryData, (int)Offsets.EquipInventoryData.UnkInventory, _hook);
            Unk2Items = new InventoryItemList(equipInventoryData, (int)Offsets.EquipInventoryData.Unk2Inventory, _hook);
        }

        public IntPtr Resolve() => _equipInventoryData.Resolve();
        public PHPointer GetPointer() => _equipInventoryData;
        public InventoryItemList NormalItems;
        public InventoryItemList KeyItems;
        /// Not really sure what this is
        internal InventoryItemList UnkItems;
        /// Not really sure what this is
        internal InventoryItemList Unk2Items;

        public uint TotalInventoryCap =>
            _equipInventoryData.ReadUInt32((int)Offsets.EquipInventoryData.TotalInventoryCap);

        public uint NormalInventoryEntries => NormalItems.Entries;
        public uint NormalInventoryCap => NormalItems.Cap;
        public uint KeyInventoryEntries => KeyItems.Entries;
        public uint KeyInventoryCap => KeyItems.Cap;

        public List<InventoryEntry> GetNormalInventory()
        {
            return NormalItems.GetInventoryList();
        }

        public List<InventoryEntry> GetKeyInventory()
        {
            return KeyItems.GetInventoryList();
        }
    }
}