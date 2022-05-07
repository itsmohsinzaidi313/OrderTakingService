using System.Collections.Generic;
using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class OnSpotDeal : Item
    {
        [XmlAttribute]
        public string UniqueDealId { get; set; } = string.Empty;
        public List<OnSpotDealItem> DealItems { get; set; } = new List<OnSpotDealItem>();
    }

    public class OnSpotDealItem : Item
    {
        [XmlAttribute]
        public double Choice { get; set; } = 0;
    }
}