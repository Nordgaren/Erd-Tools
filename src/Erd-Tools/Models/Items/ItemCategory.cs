using Erd_Tools;
using Erd_Tools.Models;
using Erd_Tools.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Category = Erd_Tools.Models.Item.Category;

namespace Erd_Tools.Models
{
    public class ItemCategory
    {

        public string Name;
        public List<Item> Items;
        public Category Category;
        public bool ShowID;
        public static List<ItemCategory> All = new();

        private static Regex CategoryEntryRx = new(@"^(?<category>\S+) (?<show>\S+) (?<path>\S+) (?<name>.+)$", RegexOptions.CultureInvariant);

        private ItemCategory(string name, Category category, string[] itemList, bool showIDs)
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

        private void AddItem(string line, Category category, bool showIDs)
        {
            switch (category)
            {
                case Category.Weapons:
                    Items.Add(new Weapon(line, category, showIDs));
                    break;
                case Category.Protector:
                case Category.Accessory:
                case Category.Goods:
                    Items.Add(new Item(line, category, showIDs));
                    break;
                case Category.Gem:
                    Items.Add(new Gem(line, category, showIDs));
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
                Category category = (Category)Convert.ToUInt32(cat, 16);
                string path = itemEntry.Groups["path"].Value;
                All.Add(new(name, category, Util.GetListResource($"Resources/{path}"), show));
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
