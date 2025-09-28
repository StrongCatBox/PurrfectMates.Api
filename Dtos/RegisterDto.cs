using System.ComponentModel.DataAnnotations;
using PurrfectMates.Enums;

namespace PurrfectMates.Api.Dtos
{
    public class RegisterDto
    {
        [Required, MaxLength(50)]
        public string Nom { get; set; } = "";

        [Required, MaxLength(50)]
        public string Prenom { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string MotDePasse { get; set; } = "";


        public Role Role { get; set; } = Role.Adoptant;

        public string? PhotoProfilUtilisateur { get; set; } 
    }
}
