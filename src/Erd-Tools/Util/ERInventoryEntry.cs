using System;
using static Erd_Tools.ERItem;

namespace Erd_Tools
{
    public class ERInventoryEntry
    {
        public byte[] Bytes { get;  }
        public string Name { get;  }
        public int GaItemHandle { get; }
        public int ItemID { get; }
        public Category Category { get; }
        public int Quantity {  get; }
        public int DisplayID { get; }

        public ERInventoryEntry(byte[] bytes, ERHook hook)
        {
            Bytes = bytes;
            Name = "Unknown";
            GaItemHandle = BitConverter.ToInt32(Bytes, (int)EROffsets.InventoryEntry.GaItemHandle);

            var buffer = new byte[4];
            Array.Copy(Bytes, (int)EROffsets.InventoryEntry.ItemID, buffer, 0, buffer.Length);
            buffer[3] &= 0xF;
            ItemID = BitConverter.ToInt32(buffer);

            var cat = Bytes[(int)EROffsets.InventoryEntry.ItemCategory];
            byte mask = 0xF0;
            cat &= mask;
            Category =  (Category)(cat * 0x1000000);

            Quantity = BitConverter.ToInt32(Bytes, (int)EROffsets.InventoryEntry.Quantity);

            DisplayID = BitConverter.ToInt32(Bytes, (int)EROffsets.InventoryEntry.DispalyID);

            switch (Category)
            {
                case Category.Weapons:
                    var id = Util.DeleteFromEnd(ItemID, 2) * 100;
                    var upgradeLevel = ItemID - id;
                    var levelString = upgradeLevel > 0 ? $"+{upgradeLevel.ToString() }" : "";
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
