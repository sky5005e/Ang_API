using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Models
{
    public class DistributionModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public decimal Amount { get; set; }
        public int FundId { get; set; }
        public int InvestorId { get; set; }
        public decimal InvestorAmount { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; } 
        
    }
}