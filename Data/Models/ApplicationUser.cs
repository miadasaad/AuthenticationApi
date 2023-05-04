using Microsoft.AspNetCore.Identity;

namespace Lab3.Data.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Address { get; set; }

    }
}
