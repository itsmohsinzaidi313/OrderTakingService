using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OrderTakingService.Models
{
    public class Table
    {
        [XmlAttribute]
        public string id { get; set; } = string.Empty;
        [XmlAttribute]
        public string tableName { get; set; } = string.Empty;
        [XmlAttribute]
        public bool reserved { get; set; }
    }
}