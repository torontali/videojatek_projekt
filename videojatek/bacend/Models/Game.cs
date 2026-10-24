namespace backend.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string ReleaseDate { get; set; }
        public string CoverImage { get; set; }
        public int Votes { get; set; }
        public string Platform { get; set; }
        public string Genre { get; set; }
        public string VotesText => $"{Votes} szavazat";

    }
}
