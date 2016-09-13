using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSFinancial.Data; 

namespace BSFinancial.Models
{
    public class LoanPropertyModel
    {
        public int LoanId { get; set; }
        public List<PropertyModel> Properties { get; set; }
    }

    public class PropertyModel
    {
        public int Id { get; set; }
        public string ProjectStatus { get; set; }
        public string LegalDescription { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public decimal BuildSquareFt { get; set; }
        public decimal LotSquareFt { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal AskingPrice { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
     
    }

    public class ViewedPropertyModel
    {
        public int Id { get; set; }
        public int proposalId { get; set; }
        public string ProjectStatus { get; set; }
        public string LegalDescription { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime LastViewDate { get; set; }
        public int ViewedDuration { get; set; }
        public int TotalViewed { get; set; }
        public PropertyPhoto photo { get; set; }
    }
}