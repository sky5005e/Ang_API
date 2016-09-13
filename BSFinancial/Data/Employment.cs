using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Employment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }
        public string Position { get; set; }
        public int HourlySalary{ get; set; }
        public int AnnualIncome { get; set; }

        public int ApplicantId { get; set; }
        public int ApplicationId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }

        public Applicant applicant { get; set; }
    }
}