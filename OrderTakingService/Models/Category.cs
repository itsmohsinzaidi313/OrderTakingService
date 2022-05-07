using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class Category
    {
        public Category()
        {

        }

        public Category(string id, string name)
        {
            Id = id;
            Name = name;
        }
        [XmlAttribute]
        public string Id { get; set; } = string.Empty;
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;
    }
}