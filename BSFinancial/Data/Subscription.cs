using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Subscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompanyId { get; set; }  
        public string PlanType { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsAllowedDegrade { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int CountryId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public long PaymentTokenNo { get; set; }

    }
}