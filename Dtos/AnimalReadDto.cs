namespace PurrfectMates.Api.Dtos
{
    public class AnimalReadDto
    {
        public int IdAnimal { get; set; }
        public string nomAnimal { get; set; } = default!;
        public string race { get; set; } = default!;
        public int age { get; set; }
        public int IdUtilisateur { get; set; }
        public int IdNiveauActivite { get; set; }
        public int IdTailleAnimal { get; set; }
        public int IdTypeAnimal { get; set; }
        public string? descriptionAnimal { get; set; }
    }
}
