using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class FundInvestor
    {
        public int Id { get; set; }
        public int FundId { get; set; }
        public int InvestorId { get; set; }
        public long Shares { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}