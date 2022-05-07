using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class Customer
    {
        [XmlAttribute]
        public string Id { get; set; } = string.Empty;
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;
        [XmlAttribute]
        public string Contact { get; set; } = string.Empty;
        [XmlAttribute]
        public string Address { get; set; } = string.Empty;
    }
}