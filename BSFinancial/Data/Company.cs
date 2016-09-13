using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFinancial.Data
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<ContactModel> Inquiries { get; set; }
        public ICollection<Applicant> Applicants { get; set; }
        public ICollection<Loan> Loans { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
