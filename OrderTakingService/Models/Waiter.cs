using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class Waiter
    {
        [XmlAttribute]
        public string id { get; set; } = string.Empty;
        [XmlAttribute]
        public string name { get; set; } = string.Empty;
    }
}