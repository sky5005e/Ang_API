using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSFinancial.Data
{
    public class PropertyValue
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public decimal MarketValue { get; set; }
        public int PropertyId { get; set; }
    }
}
