using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly Application _context;

        public PlayersController(Application context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var players = _context.Players.OrderBy(player => player.Id).ToList();
            return Ok(players);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var player = _context.Players.FirstOrDefault(item => item.Id == id);
            return player is null ? NotFound("A játékos nem található.") : Ok(player);
        }

        [HttpPost]
        public IActionResult Create([FromBody] player input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Email))
                return BadRequest("A felhasználónév és az e-mail-cím kötelező.");

            var newPlayer = new player
            {
                Username = input.Username.Trim(),
                Email = input.Email.Trim(),
                Level = Math.Max(1, input.Level)
            };

            try
            {
                _context.Players.Add(newPlayer);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = newPlayer.Id }, newPlayer);
            }
            catch (DbUpdateException ex) when (IsDuplicateKey(ex))
            {
                return Conflict("A felhasználónévnek és az e-mail-címnek egyedinek kell lennie.");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] player input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Email))
                return BadRequest("A felhasználónév és az e-mail-cím kötelező.");

            var existingPlayer = _context.Players.FirstOrDefault(item => item.Id == id);
            if (existingPlayer is null)
                return NotFound("A játékos nem található.");

            existingPlayer.Username = input.Username.Trim();
            existingPlayer.Email = input.Email.Trim();
            existingPlayer.Level = Math.Max(1, input.Level);

            try
            {
                _context.SaveChanges();
                return NoContent();
            }
            catch (DbUpdateException ex) when (IsDuplicateKey(ex))
            {
                return Conflict("A felhasználónévnek és az e-mail-címnek egyedinek kell lennie.");
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var player = _context.Players.Find(id);
            if (player is null)
                return NotFound("A játékos nem található.");

            _context.Players.Remove(player);
            _context.SaveChanges();
            return NoContent();
        }

        // Saját függvény: lekérdezi, hogy egy játékos mely játékokat értékelte.
        [HttpGet("{id:int}/games")]
        public IActionResult GetRatedGames(int id)
        {
            bool playerExists = _context.Players.Any(player => player.Id == id);
            if (!playerExists)
                return NotFound("A játékos nem található.");

            var ratedGames = _context.Reviews
                .Where(review => review.PlayerId == id)
                .Join(_context.Games,
                    review => review.GameId,
                    game => game.Id,
                    (review, game) => new
                    {
                        GameId = game.Id,
                        GameName = game.Name,
                        review.Points,
                        review.Comment,
                        review.ReviewReleaseDate
                    })
                .OrderByDescending(item => item.ReviewReleaseDate)
                .ToList();

            return Ok(ratedGames);
        }

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
}
