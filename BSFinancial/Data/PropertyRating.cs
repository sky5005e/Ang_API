using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class PropertyRating
    {
        public int Id { get; set; }
        public int propertyId { get; set; }
        public int location { get; set; }
        public int cost { get; set; }
        public int overall { get; set; }
    }
}