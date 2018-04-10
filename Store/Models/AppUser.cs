using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public enum MembershipTypes
    {
        Gold, Silver, Bronze
    }

    public class AppUser : IdentityUser
    {
        public MembershipTypes? MembershipType { get; set; }
        public DateTime? ExprireDate { get; set; }
        public String Message { get; set; }
    }
}
