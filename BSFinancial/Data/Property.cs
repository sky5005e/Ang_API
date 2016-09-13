using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFinancial.Data
{
    public class Property
    {
        public int Id { get; set; }
        public string ProjectStatus { get; set; }
        public string LegalDescription { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public decimal ListPrice { get; set; }
        public decimal OfferPrice { get; set; }
        public DateTime? OfferDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal TaxAmount { get; set; }
        public ICollection<PropertyValue> HCADValues { get; set; }
        public int CompanyId { get; set; }
        public int? LoanId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public decimal BuildSquareFt { get; set; }
        public decimal LotSquareFt { get; set; }
        public decimal AskingPrice { get; set; }
        public DateTime? LastPayment { get; set; }
        public ICollection<PropertyDocument> Documents { get; set; }
        public ICollection<PropertyPhoto> Photos { get; set; }
       
    }
}
