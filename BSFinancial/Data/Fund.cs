using BSFinancial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Fund
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string FundName { get; set; }
        public string TaxId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long SharesAllowed { get; set; }
        public object FundInvestors { get; set; }
        public object FundDistributions { get; set; }
 
    }
}