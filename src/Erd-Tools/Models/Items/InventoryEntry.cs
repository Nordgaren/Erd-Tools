using System;
using static Erd_Tools.Models.Item;

namespace Erd_Tools.Models
{
    public class InventoryEntry
    {
        public byte[] Bytes { get;  }
        public string Name { get;  }
        public int GaItemHandle { get; }
        public int ItemID { get; }
        public Category Category { get; }
        public int Quantity {  get; }
        public int DisplayID { get; }

        public InventoryEntry(byte[] bytes, ErdHook hook)
        {
            Bytes = bytes;
            Name = "Unknown";
            GaItemHandle = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.GaItemHandle);

            byte[] buffer = new byte[4];
            Array.Copy(Bytes, (int)Offsets.InventoryEntry.ItemID, buffer, 0, buffer.Length);
            buffer[3] &= 0xF;
            ItemID = BitConverter.ToInt32(buffer);

            byte cat = Bytes[(int)Offsets.InventoryEntry.ItemCategory];
            byte mask = 0xF0;
            cat &= mask;
            Category =  (Category)(cat * 0x1000000);

            Quantity = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.Quantity);

            DisplayID = BitConverter.ToInt32(Bytes, (int)Offsets.InventoryEntry.DispalyID);

            switch (Category)
            {
                case Category.Weapons:
                    int id = Util.DeleteFromEnd(ItemID, 2) * 100;
                    int upgradeLevel = ItemID - id;
                    string levelString = upgradeLevel > 0 ? $"+{ upgradeLevel }" : "";
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
                    break;
            }
        }
    }
}
