namespace backend.Models
{
    // A Games táblában a TypeId erre a műfajtáblára hivatkozik.
    public class GameType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
