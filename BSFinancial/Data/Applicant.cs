using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BSFinancial.Data
{
    public class Applicant
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public virtual string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
            set
            {
            }
        }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        public string Ssn { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
//         public ApplicantAddr CurrAddress { get; set; }
//         public ApplicantAddr PrevAddress { get; set; }

        [Required]
        public int CompanyId { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }

//         public ICollection<Application> Applicants { get; set; }
//         public ICollection<Application> CoApplicants { get; set; }

        }
    }
