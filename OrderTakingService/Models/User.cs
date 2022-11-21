using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class User
    {
        [XmlAttribute]
        public string Id { get; set; } = string.Empty;
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;
    }
}