using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Address { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal PrincipalAmt { get; set; }
        public decimal InterestAmt { get; set; }
        public decimal EscrowAmt { get; set; }
        public DateTime PayDate { get; set; }
        public int LoanId { get; set; }
        public int CompanyId { get; set; }
    }
}