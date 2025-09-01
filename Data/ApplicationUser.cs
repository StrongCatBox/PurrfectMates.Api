using Microsoft.AspNetCore.Identity;

namespace PurrfectMates.Api.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Affichage { get; set; }
    }
}
