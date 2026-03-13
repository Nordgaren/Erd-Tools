using Erd_Tools;
using Erd_Tools.Models;
using Erd_Tools.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GoodsCategory = Erd_Tools.Models.Item.Category;

namespace Erd_Tools.Models
{
    public class ItemCategory
    {

        public string Name;
        public List<Item> Items;
        public GoodsCategory Category;
        public bool ShowID;
        public static List<ItemCategory> All = new();
        public static List<ItemCategory> Live = new();

        private static Regex CategoryEntryRx = new(@"^(?<category>\S+) (?<show>\S+) (?<path>\S+) (?<name>.+)$", RegexOptions.CultureInvariant);

        private ItemCategory(string name, GoodsCategory category, string[] itemList, bool showIDs)
        {
            Name = name;
            Items = new();
            Category = category;
            foreach (string line in itemList)
            {
                if (Util.IsValidTxtResource(line)) //determine if line is a valid resource or not
                    AddItem(line.TrimComment(), category, showIDs);
            };
        }
        
        public ItemCategory(string name, GoodsCategory category, List<Item> items)
        {
            Name = name;
            Category = category;
            Items = items;
        }

        private void AddItem(string line, GoodsCategory goodsCategory, bool showIDs)
        {
            switch (goodsCategory)
            {
                case GoodsCategory.Weapons:
                    Items.Add(new Weapon(line, goodsCategory, showIDs));
                    break;
                case GoodsCategory.Protector:
                case GoodsCategory.Accessory:
                case GoodsCategory.Goods:
                    Items.Add(new Item(line, goodsCategory, showIDs));
                    break;
                case GoodsCategory.Gem:
                    Items.Add(new Gem(line, goodsCategory, showIDs));
                    break;
                default:
                    throw new Exception("Incorrect Category");
            }
        }

        public static void GetItemCategories()
        {
            string[] result = Util.GetListResource("Resources/ItemCategories.txt");
            All = new();

            foreach (string line in result)
            {
                if (!Util.IsValidTxtResource(line)) //determine if line is a valid resource or not
                    continue;

                Match itemEntry = CategoryEntryRx.Match(line.TrimComment());
                string name = itemEntry.Groups["name"].Value;
                bool show = Convert.ToBoolean(itemEntry.Groups["show"].Value);
                string cat = itemEntry.Groups["category"].Value;
                GoodsCategory category = (GoodsCategory)Convert.ToUInt32(cat, 16);
                string path = itemEntry.Groups["path"].Value;
                All.Add(new(name, category, Util.GetListResource($"Resources/{path}"), show));
            };
        }
        
        public static void GetInMemoryItemCategories(ErdHook hook)
        {
            List<Item> items = new();

            foreach (Item item in items)
            {
               
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
