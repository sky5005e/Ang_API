using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Investor
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public virtual string FullName
        {
            get
            { return FirstName + " " + LastName; }
            set { }
        }

        public string Ssn { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public decimal AmountPaid { get; set; }
        public long Shares { get; set; }
       
    }
}