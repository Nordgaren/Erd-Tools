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
        List<InventoryEntry> _normalInventory;
        List<InventoryEntry> _keyInventory;
        
        public EquipInventoryData(PHPointer equipInventoryData, ErdHook hook)
        {
            _equipInventoryData = equipInventoryData;
            _hook = hook;
            _normalItems = _hook.CreateChildPointer(equipInventoryData, (int)Offsets.EquipInventoryData.NormalInventoryOffset);
            _keyItems = _hook.CreateChildPointer(equipInventoryData, (int)Offsets.EquipInventoryData.KeyInventoryOffset);
        }
        public IntPtr Resolve() => _equipInventoryData.Resolve();
        public PHPointer GetPointer() => _equipInventoryData;
        public uint TotalInventoryCap =>
            _equipInventoryData.ReadUInt32((int)Offsets.EquipInventoryData.TotalInventoryCap);
        public uint NormalInventoryEntries =>
            _equipInventoryData.ReadUInt32((int)Offsets.EquipInventoryData.NormalInventoryCount);
        public uint NormalInventoryCap =>
            _equipInventoryData.ReadUInt32((int)Offsets.EquipInventoryData.NormalInventoryCap);
        public uint KeyInventoryEntries =>
            _equipInventoryData.ReadUInt32((int)Offsets.EquipInventoryData.KeyInventoryCount);
        public uint KeyInventoryCap =>
            _equipInventoryData.ReadUInt32((int)Offsets.EquipInventoryData.KeyInventoryCap);
        
        public List<InventoryEntry> GetNormalInventory()
        {
            return GetInventoryList();
        }

        public List<InventoryEntry> GetKeyInventory()
        {
            return GetStorageList();
        }

        private List<InventoryEntry> GetInventoryList()
        {
            return GetInventoryList(_normalItems, NormalInventoryEntries, NormalInventoryCap);
        }

        private List<InventoryEntry> GetStorageList()
        {
            return GetInventoryList(_keyItems, KeyInventoryEntries, KeyInventoryCap);
        }

        private List<InventoryEntry> GetInventoryList(PHPointer inventoryPointer, uint inventoryEntries, uint inventoryCap)
        {
            byte[] bytes = inventoryPointer.ReadBytes(0x0, inventoryCap * Offsets.InventoryEntrySize);
            List<InventoryEntry> inventory = new();
            for (int i = 0; inventory.Count < inventoryEntries; i++)
            {
                byte[] entry = new byte[Offsets.InventoryEntrySize];
                Array.Copy(bytes, i * Offsets.InventoryEntrySize, entry, 0, entry.Length);

                if (BitConverter.ToInt32(entry, (int)Offsets.InventoryEntry.ItemID) == -1) continue;

                inventory.Add(new InventoryEntry(
                    _hook.CreateBasePointer(inventoryPointer.Resolve() + i * Offsets.InventoryEntrySize), (uint)i, _hook)
                );
            }

            return inventory;
        }

        public void ResetInventory()
        {
            _normalInventory = new List<InventoryEntry>();
        }

        public void ResetKeyInventory()
        {
            _keyInventory = new List<InventoryEntry>();
        }

    }
}