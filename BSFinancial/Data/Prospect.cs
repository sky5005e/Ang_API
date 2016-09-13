using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Prospect
    {
        public int Id { get; set; }
        public int CompanyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        //public string City { get; set; }
        //public string State { get; set; }
        //public string Zip { get; set; }
        //public DateTime ProposalDate { get; set; }
    }
}