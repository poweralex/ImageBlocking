using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlocking.Models
{
    public class Inventory
    {
        public List<InventoryUnit> Items { get; set; } = new List<InventoryUnit>();
    }

    public class InventoryUnit
    {
        public Block Block { get; set; }
        public int Qty { get; set; }
    }
}
