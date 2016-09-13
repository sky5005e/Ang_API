using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class PropertyViewInfo
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int ProposalId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime LastViewDate { get; set; }
        public int ViewDuration { get; set; }
    }
}