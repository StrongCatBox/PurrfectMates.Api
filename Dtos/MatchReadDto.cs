namespace PurrfectMates.Api.Dtos
{
    public class MatchReadDto
    {
        public int UtilisateurId { get; set; }
        public int AnimalId { get; set; }
        public DateTime DateMatch { get; set; }

        public AnimalDto Animal { get; set; } = new AnimalDto();
        public UtilisateurDto? Utilisateur { get; set; } // utile pour les propriétaires
    }

    public class AnimalDto
    {
        public string NomAnimal { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public int Age { get; set; }
        public string DescriptionAnimal { get; set; } = string.Empty;
    }

    public class UtilisateurDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
