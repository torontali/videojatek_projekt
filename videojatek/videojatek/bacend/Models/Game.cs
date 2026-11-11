using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    // Az adatbázis Games táblájának modellje, valamint az API válasz néhány számított mezője.
    public class Game
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public int Votes { get; set; }
        public string? Platform { get; set; }

        public int? TypeId { get; set; }
        [JsonIgnore]
        public GameType? GameType { get; set; }

        // Ezek API-megjelenítési mezők, nem önálló adatbázisoszlopok.
        [NotMapped]
        public string? Type { get; set; }

        [NotMapped]
        public string? Categories { get; set; }

        [NotMapped]
        public string VotesText => $"{Votes} szavazat";
    }
}
