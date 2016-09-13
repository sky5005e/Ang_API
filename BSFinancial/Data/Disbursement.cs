using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Disbursement
    {
        public int Id { get; set; }
        public DateTime PayDate { get; set; }
        //public decimal EscrowAmt { get; set; }
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public int LoanId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
   
    }
}