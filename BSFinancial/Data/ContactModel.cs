using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BSFinancial.Data
{
    public class ContactModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public string Name { get; set; }
        public string LoanType { get; set; }
        public string PropertyCost { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CurrentAddress { get; set; }
        public string RentOwn { get; set; }
        public int MonthlyRent { get; set; }
        public string EmployedBy { get; set; }
        public int MonthlyIncome { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CompanyId { get; set; }  

    }
}
