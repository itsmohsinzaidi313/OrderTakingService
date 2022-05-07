using System.Collections.Generic;

namespace OrderTakingService.Models
{
    public class FixedDeal : Item
    {
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}