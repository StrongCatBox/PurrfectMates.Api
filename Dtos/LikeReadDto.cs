namespace PurrfectMates.Api.Dtos
{
    public class LikeReadDto
    {
        public int IdSwipe { get; set; }
        public int AnimalId { get; set; }
        public string ActionSwipe { get; set; } = string.Empty;
        public DateTime DateSwipe { get; set; }
    }
}
