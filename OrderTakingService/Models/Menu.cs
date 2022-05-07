using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderTakingService.Models
{
    public class Menu
    {
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<FixedDeal> FixedDeals { get; set; } = new List<FixedDeal>();
        public List<OnSpotDeal> OnSpotDeals { get; set; } = new List<OnSpotDeal>();

        public bool IsValid
        {
            get
            {
                Items = Items ?? new List<MenuItem>();
                Categories = Categories ?? new List<Category>();

                if (Items.Count == 0 || Categories.Count == 0)
                    return false;

                return true;
            }
        }
    }
}