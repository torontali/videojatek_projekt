namespace backend.Models
{
    public class Rewiev
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int GameId { get; set; }
        public int Points { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewReleaseDate { get; set; }
    }
}
