using PropertyHook;
using System;
using System.Text;

namespace Erd_Tools.Models.Game
{
    /// <summary>
    /// PlayerGameData class which handles the games PlayerGameData structure and related functionality. This includes
    /// the players name, runes held and lifetimes runes, inventory and storage, gestures and more!
    /// </summary>
    public class PlayerGameData
    {
        private ErdHook _hook;
        private PHPointer _playerGameData;
        private PHPointer _heldNormalItemsPtr;
        private PHPointer _heldSpecialItemsPtr;
        private bool Loaded => _hook.Loaded;

        public PlayerGameData(PHPointer playerGameData, ErdHook hook)
        {
            _playerGameData = playerGameData;
            _hook = hook;
            Inventory =
                new EquipInventoryData(_hook.CreateChildPointer(_playerGameData, Offsets.EquipInventoryDataOffset),
                    _hook);
            Storage = new EquipInventoryData(_hook.CreateChildPointer(_playerGameData, Offsets.EquipStorageDataOffset),
                _hook);

            _heldNormalItemsPtr = _hook.CreateChildPointer(_playerGameData,
                (int)Offsets.PlayerGameData.HeldNormalItems);
            _heldSpecialItemsPtr =
                _hook.CreateChildPointer(_playerGameData, (int)Offsets.PlayerGameData.HeldSpecialItems);
        }
        
        public IntPtr Resolve() => _playerGameData.Resolve();
        public PHPointer GetPointer() => _playerGameData;
        
        public string Name
        {
            get =>
                _playerGameData.ReadString((int)Offsets.PlayerGameData.Name, Encoding.Unicode,
                    32);
            set
            {
                if (value.Length > 16) return;

                _playerGameData.WriteString((int)Offsets.PlayerGameData.Name, Encoding.Unicode,
                    32, value);
            }
        }
        
        public int Level => _playerGameData.ReadInt32((int)Offsets.Player.Level);
        public string LevelString => _playerGameData?.ReadInt32((int)Offsets.Player.Level).ToString() ?? "";
        
        public EquipInventoryData Inventory;
        public EquipInventoryData Storage;
        
        public uint InventoryLength => _playerGameData.ReadUInt32((int)Offsets.PlayerGameData.MaximumNormalItems);
        
        public int HeldNormalItems
        {
            get => _heldNormalItemsPtr.ReadInt32(0x0);
            set => _heldNormalItemsPtr.WriteInt32(0x0, value);
        }

        public int HeldSpecialItems
        {
            get => _heldSpecialItemsPtr.ReadInt32(0x0);
            set => _heldSpecialItemsPtr.WriteInt32(0x0, value);
        }
        
        public byte ArmStyle
        {
            get => _playerGameData.ReadByte((int)Offsets.ChrIns.ArmStyle);
            set
            {
                if (!_hook.Loaded) return;
                _playerGameData.WriteByte((int)Offsets.ChrIns.ArmStyle, value);
            }
        }

        public int CurrWepSlotOffsetLeft
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.CurrWepSlotOffsetLeft);
            set
            {
                if (!_hook.Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.CurrWepSlotOffsetLeft, value);
            }
        }

        public int CurrWepSlotOffsetRight
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.CurrWepSlotOffsetRight);
            set
            {
                if (!_hook.Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.CurrWepSlotOffsetRight, value);
            }
        }

        public int RHandWeapon1
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.RHandWeapon1);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.RHandWeapon1, value);
            }
        }

        public int RHandWeapon2
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.RHandWeapon2);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.RHandWeapon2, value);
            }
        }

        public int RHandWeapon3
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.RHandWeapon3);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.RHandWeapon3, value);
            }
        }

        public int LHandWeapon1
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.LHandWeapon1);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.LHandWeapon1, value);
            }
        }

        public int LHandWeapon2
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.LHandWeapon2);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.LHandWeapon2, value);
            }
        }

        public int LHandWeapon3
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.LHandWeapon3);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.LHandWeapon3, value);
            }
        }

        public int Arrow1
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.Arrow1);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.Arrow1, value);
            }
        }

        public int Arrow2
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.Arrow2);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.Arrow2, value);
            }
        }

        public int Bolt1
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.Bolt1);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.Bolt1, value);
            }
        }

        public int Bolt2
        {
            get => _playerGameData.ReadInt32((int)Offsets.ChrIns.Bolt2);
            set
            {
                if (!Loaded) return;
                _playerGameData.WriteInt32((int)Offsets.ChrIns.Bolt2, value);
            }
        }
        

    }
}