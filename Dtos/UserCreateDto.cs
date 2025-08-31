using PurrfectMates.Enums;

namespace PurrfectMates.Api.Dtos
{
    public class UserCreateDto
    {
        public string nomUtilisateur { get; set; }
        public string prenomUtilisateur { get; set; }
        public string emailUtilisateur { get; set; }
        public string motDePasseUtilisateur { get; set; }
        public Role Role { get; set; }   // ⚡ majuscule
        public string? photoProfilUtilisateur { get; set; }
    }
}
