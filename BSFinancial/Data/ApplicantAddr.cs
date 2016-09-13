using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class ApplicantAddr
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
        public string OwnRent { get; set; }
        public int MonthlyPay { get; set; }
        public int HowLong { get; set; }

        public int ApplicantId { get; set; }
        public int ApplicationId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }

        public Applicant applicant { get; set; }
    }
}