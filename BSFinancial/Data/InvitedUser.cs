using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFinancial.Data
{
    public class InvitedUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int InvitedCompId { get; set; }
        public Boolean Status { get; set; }
        public  Boolean IsDeleted { get; set; }
        public string EmailToken { get; set; }

    }
}