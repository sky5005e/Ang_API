using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class ProposalProperty
    {
        public int Id { get; set; }
        public int ProposalId { get; set; }
        public int PropertyID { get; set; }
        public int CompanyID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}