using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal PrincipalAmt { get; set; }
        public decimal InterestAmt { get; set; }
        public decimal EscrowAmt { get; set; }
        public DateTime PayDate { get; set; }
        public int LoanId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}