using Microsoft.AspNetCore.Identity;

namespace EXAMPORTAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        // Role will be managed via Identity roles ("User", "Admin")
    }
   
}
