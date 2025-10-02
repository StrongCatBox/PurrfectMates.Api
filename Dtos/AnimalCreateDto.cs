namespace PurrfectMates.Api.Dtos
{
    public class AnimalCreateDto
    {
        public string nomAnimal { get; set; } = string.Empty;
        public string race { get; set; } = string.Empty;
        public int age { get; set; }
        public string? descriptionAnimal { get; set; }
        public int typeAnimalId { get; set; }
        public int tailleAnimalId { get; set; }
        public int niveauActiviteId { get; set; }
        public List<int> TemperamentIds { get; set; } = new();
        public List<int> TypeLogementIds { get; set; } = new();
    }
}
