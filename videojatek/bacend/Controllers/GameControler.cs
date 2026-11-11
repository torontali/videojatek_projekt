using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly Application _context;

        public GamesController(Application context) => _context = context;

        [HttpGet]
        public IActionResult GetAllGames()
        {
            var games = _context.Games
                .Include(game => game.GameType)
                .OrderBy(game => game.Id)
                .ToList()
                .Select(ToApiGame);

            return Ok(games);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetGameById(int id)
        {
            var game = _context.Games
                .Include(item => item.GameType)
                .FirstOrDefault(item => item.Id == id);

            return game is null
                ? NotFound("A játék nem található.")
                : Ok(ToApiGame(game));
        }

        [HttpPost]
        public IActionResult AddGame([FromBody] GameInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
                return BadRequest("A játék neve kötelező.");

            try
            {
                var game = new Game
                {
                    Name = input.Name.Trim(),
                    Company = Clean(input.Company),
                    ReleaseDate = Clean(input.ReleaseDate),
                    CoverImage = Clean(input.CoverImage),
                    Platform = Clean(input.Platform),
                    Votes = 0,
                    TypeId = GetOrCreateTypeId(input.Type)
                };

                _context.Games.Add(game);
                _context.SaveChanges();

                var savedGame = _context.Games
                    .Include(item => item.GameType)
                    .First(item => item.Id == game.Id);

                return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, ToApiGame(savedGame));
            }
            catch (DbUpdateException ex) when (IsDuplicateKey(ex))
            {
                return Conflict("Ez a műfaj már szerepel az adatbázisban.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Játék mentési hiba: {ex}");
                return StatusCode(500, "Nem sikerült menteni a játékot.");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateGame(int id, [FromBody] GameInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
                return BadRequest("A játék neve kötelező.");

            var game = _context.Games.FirstOrDefault(item => item.Id == id);
            if (game is null)
                return NotFound("A játék nem található.");

            game.Name = input.Name.Trim();
            game.Company = Clean(input.Company);
            game.ReleaseDate = Clean(input.ReleaseDate);
            game.CoverImage = Clean(input.CoverImage);
            game.Platform = Clean(input.Platform);
            game.TypeId = GetOrCreateTypeId(input.Type);

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteGame(int id)
        {
            var game = _context.Games.Find(id);
            if (game is null)
                return NotFound("A játék nem található.");

            _context.Games.Remove(game);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("platforms")]
        public IActionResult GetPlatforms()
        {
            var platforms = _context.Games
                .Where(game => game.Platform != null && game.Platform.Trim() != "")
                .Select(game => game.Platform!)
                .Distinct()
                .OrderBy(platform => platform)
                .ToList();

            return Ok(platforms);
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _context.Types
                .Where(type => type.Name != null && type.Name.Trim() != "")
                .OrderBy(type => type.Name)
                .Select(type => type.Name)
                .ToList();

            return Ok(categories);
        }

        // Saját függvény: keresés név, kiadó vagy platform szerint.
        [HttpGet("search")]
        public IActionResult SearchGames([FromQuery] string? q)
        {
            string term = q?.Trim() ?? string.Empty;

            var games = _context.Games
                .Include(game => game.GameType)
                .Where(game =>
                    (game.Name != null && game.Name.Contains(term)) ||
                    (game.Company != null && game.Company.Contains(term)) ||
                    (game.Platform != null && game.Platform.Contains(term)))
                .OrderBy(game => game.Name)
                .ToList()
                .Select(ToApiGame);

            return Ok(games);
        }

        private int? GetOrCreateTypeId(string? typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            string name = typeName.Trim();
            var existingType = _context.Types.FirstOrDefault(type => type.Name == name);
            if (existingType is not null)
                return existingType.Id;

            var newType = new GameType { Name = name };
            _context.Types.Add(newType);
            _context.SaveChanges();
            return newType.Id;
        }

        private static Game ToApiGame(Game game)
        {
            game.Type = game.GameType?.Name;
            game.Categories = game.GameType?.Name;
            return game;
        }

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static bool IsDuplicateKey(Exception exception)
        {
            for (Exception? current = exception; current is not null; current = current.InnerException)
            {
                if (current.Message.Contains("Duplicate", StringComparison.OrdinalIgnoreCase) ||
                    current.Message.Contains("1062", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }

    public class GameInput
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public string? Platform { get; set; }
        public string? Type { get; set; }
    }
}
