using System;
using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class Item
    {
        public Item()
        {
        }       
        [XmlAttribute]
        public string RowId { get; set; }
        [XmlAttribute]
        public string Id { get; set; } = string.Empty;
        [XmlAttribute]
        public string Code { get; set; } = string.Empty;
        [XmlAttribute]
        public string CategoryId { get; set; } = string.Empty;
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;
        [XmlAttribute]
        public double Price { get; set; } = 0;
        [XmlAttribute]
        public double Quantity { get; set; } = 0;
        [XmlAttribute]
        public double TaxAmount { get; set; } = 0;
        [XmlAttribute]
        public string Comment { get; set; } = string.Empty;
        [XmlAttribute]
        public double Amount { get; set; } = 0;
        [XmlAttribute]
        public bool Selected { get; set; } = false;
        [XmlAttribute]
        public bool IsAdditional { get; set; } = false;
    }
}