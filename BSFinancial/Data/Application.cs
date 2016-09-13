using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class Application
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationName { get; set; }
        //public ICollection<Loan> Loans { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantIds { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }

        [NotMapped]
        public IList<int> ApplicantIdList
        {
            get
            {
                if (String.IsNullOrWhiteSpace(ApplicantIds)) {
                    return new List<int>();
                } else {
                    return ApplicantIds.Split(',').Select(int.Parse).ToList();
                }

            }
            set
            {
                var strlist = value.ToList().ConvertAll(s => s.ToString());
                ApplicantIds = String.Join(",", strlist.ToArray());
            }
        }

        [NotMapped]
        public virtual Applicant MainApplicant { get; set; }
        [NotMapped]
        public virtual ICollection<Applicant> Applicants { get; set; }
        [NotMapped]
        public virtual ICollection<Employment> Employments { get; set; }
        [NotMapped]
        public virtual ICollection<ApplicantAddr> Addrs { get; set; }
        //         public Applicant applicant { get; set; }
//         public Applicant co_applicant { get; set; }
    }
}