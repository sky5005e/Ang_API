using BSFinancial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Models
{
    public class ProposalPropertyModel
    {
        public int ProposalId { get; set; }
        public string ProposalFor { get; set; }
        public string Prospects { get; set; }
        public string Agents { get; set; }
        public DateTime ProposalDate { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public List<PropertyModel> Properties { get; set; }
       
    }

    public class ProposalProspectModel
    {
        public int Id { get; set; }
        public int ProspectId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Agents { get; set; }
        public string ProposalFor { get; set; }
        public DateTime ProposalDate { get; set; }
        public DateTime? SentOn { get; set; }
        public string btnDisplayText { get; set; }
        public string TokenKey { get; set; }

    }
}