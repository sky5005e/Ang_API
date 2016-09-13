using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BSFinancial.Data
{
    public class Loan
    {
        public int Id { get; set; }
        //public Applicant applicant { get; set; }

        //[ForeignKey("Application_Id")]
        //public Application application { get; set; }
        public int ApplicationId { get; set; }

        public int CompanyId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Principal { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public int Months { get; set; }
        public decimal Rate { get; set; }
        public decimal EscrowTaxAmount { get; set; }
        public decimal EscrowInsAmount { get; set; }
        public decimal Equity { get; set; }
        public decimal Debt { get; set; }
        public decimal DebtRate { get; set; }
        public bool Variable { get; set; }
        public DateTime CreatedOn { get; set; }

        public Application application { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Disbursement> Disbursements { get; set; }
    }
}
