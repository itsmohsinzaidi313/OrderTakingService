using System.Collections.Generic;
using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class OnSpotDeal : Item
    {
        public List<OnSpotDealItem> DealItems { get; set; } = new List<OnSpotDealItem>();
        public List<DealStep> DealSteps { get; set; }
    }

    public class OnSpotDealItem : Item
    {
        [XmlAttribute]
        public double Choice { get; set; } = 0;
        [XmlAttribute]
        public string DealStepId { get; set; }
    }

    public class DealStep
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Limit { get; set; }
    }
}