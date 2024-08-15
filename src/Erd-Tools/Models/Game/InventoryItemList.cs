using PropertyHook;
using System;
using System.Collections.Generic;

namespace Erd_Tools.Models.Game
{
    /// <summary>
    /// InventoryItemList class that handles the games inventory lists.
    /// </summary>
    public class InventoryItemList
    {
        private readonly int _offset;
        private readonly PHPointer _equipInventoryData;
        private PHPointer _list => _hook.CreateBasePointer(_equipInventoryData.Resolve() + _offset);

        private PHPointer _inventory =>
            _hook.CreateChildPointer(_list,(int)Offsets.InventoryItemList.Pointer);
        private readonly ErdHook _hook;
        public InventoryItemList(PHPointer equipInventoryData, int offset, ErdHook hook)
        {
            _equipInventoryData = equipInventoryData;
            _hook = hook;
            _offset = offset;
        }
        public PHPointer? GetPointer()
        {
            return _equipInventoryData.Resolve() == IntPtr.Zero ? null : _list;
        }
        public IntPtr Resolve()
        {
            return _equipInventoryData.Resolve() == IntPtr.Zero ? IntPtr.Zero : _list.Resolve();
        }
        public uint Cap => _list.ReadUInt32((int)Offsets.InventoryItemList.Cap);
        public uint Entries => _list.ReadUInt32((int)Offsets.InventoryItemList.Entries);
        
        public List<InventoryEntry> GetInventoryList()
        {
            byte[] bytes = _inventory.ReadBytes(0x0, Cap * Offsets.InventoryEntrySize);
            List<InventoryEntry> inventory = new();
            for (int i = 0; inventory.Count < Entries; i++)
            {
                byte[] entry = new byte[Offsets.InventoryEntrySize];
                Array.Copy(bytes, i * Offsets.InventoryEntrySize, entry, 0, entry.Length);

                if (BitConverter.ToInt32(entry, (int)Offsets.InventoryEntry.ItemID) == -1) continue;

                inventory.Add(new InventoryEntry(
                    _hook.CreateBasePointer(_inventory.Resolve() + i * Offsets.InventoryEntrySize), (uint)i, _hook)
                );
            }

            return inventory;
        }
    }
}