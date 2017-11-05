using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HGV.Basilius.Model
{
    public class Item
    {
        public int item_id { get; set; }
        public string key { get; set; }
        public string tag { get; set; }
        public string name { get; set; }

        public int price { get; set; } // cost

        public bool combinable { get; set; }
        public bool permanent { get; set; }
        public bool stackable { get; set; }
        public bool recipe { get; set; }
        public bool droppable { get; set; }
        public bool purchasable { get; set; }
        public bool sellable { get; set; }
        public bool destructible { get; set; }  // killable
        public bool disassemblable { get; set; }
        public bool require_side_shop { get; set; }
        public bool require_secret_shop { get; set; }

        public bool requires_charges { get; set; }
        public int initial_charges { get; set; }

        public List<int> cast_range { get; set; }

        public List<double> cooldown { get; set; }
        public List<int> manacost { get; set; }

        public string quality { get; set; }

        public List<string> behaviors { get; set; }

        // public Dictionary<string, List<double>> attributes { get; set; }

        public Item()
        {
            //this.attributes = new Dictionary<string, List<double>>();

            this.behaviors = new List<string>();
            this.cast_range = new List<int>();
            this.cooldown = new List<double>();
            this.manacost = new List<int>();
        }
    }
}
