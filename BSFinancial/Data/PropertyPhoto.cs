using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class PropertyPhoto
    {
        public int Id { get; set; }
        public string description { get; set; }
        public int propertyId { get; set; }
        public string filePath { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}