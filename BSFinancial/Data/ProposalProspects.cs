using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class ProposalProspects
    {
        public int Id { get; set; }
        public int ProspectId { get; set; }
        public int UserId { get; set; }
        public string ProposalFor { get; set; }
        public DateTime ProposalDate { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public DateTime? SentOn { get; set; }
        public string TokenKey { get; set; }
        public ICollection<ProposalProperty> ProspectProperties { get; set; }
    }
}