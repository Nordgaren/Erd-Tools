using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erd_Tools.Models.Item;

namespace Erd_Tools.Models.Items
{
    public class ItemSpawnInfo
    {
        public int ID { get; }
        public Category Category { get; }
        public int Quantity { get;  }
        public int MaxQuantity { get; }
        public int Infusion { get;  }
        public int Upgrade { get;  }
        public int Gem { get;  }
        public int EventID { get;  }
        public ItemSpawnInfo(int id, Category category, int quantity, int maxQuantity, int infusion, int upgrade, int gem = -1, int eventId = -1)
        {
            ID = id;
            Category = category;
            Quantity = quantity;
            MaxQuantity = maxQuantity;
            Infusion = infusion;
            Upgrade = upgrade;
            Gem = gem;
            EventID = eventId;
        }
    }
}
