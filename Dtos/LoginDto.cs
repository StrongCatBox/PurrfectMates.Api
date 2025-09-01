using System.ComponentModel.DataAnnotations;

namespace PurrfectMates.Api.Dtos
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string MotDePasse { get; set; } = "";
    }
}
