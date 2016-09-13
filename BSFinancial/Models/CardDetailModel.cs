using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Models
{
    public class CardDetailModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiryMonth { get; set; }
        public string CardExpiryYear { get; set; }

        public string EmailAddress { get; set; }
        public int CountryId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public string PaymentTokenNo { get; set; }
    }
}