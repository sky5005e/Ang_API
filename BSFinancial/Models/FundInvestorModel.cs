using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Models
{
    public class FundInvestorModel
    {
        public int FundId { get; set; }
        public List<InvestorModel> Investors { get; set; }
    }

    public class FundInvModel
    {
        public int FundId { get; set; }
        public InvestorModel Investor { get; set; }
    }
    public class InvestorModel
    {
        public int FundInvestorId { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Ssn { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public long Shares { get; set; }
        public decimal AmountPaid { get; set; }
        
       
    }
}