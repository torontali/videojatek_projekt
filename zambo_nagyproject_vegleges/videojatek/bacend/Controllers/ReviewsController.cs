using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly Application _context;

        public ReviewsController(Application context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var reviews = _context.Reviews
                .OrderByDescending(review => review.ReviewReleaseDate)
                .ToList();
            return Ok(reviews);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var review = _context.Reviews.FirstOrDefault(item => item.Id == id);
            return review is null ? NotFound("Az értékelés nem található.") : Ok(review);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Rewiev input)
        {
            var validation = Validate(input);
            if (validation is not null)
                return BadRequest(validation);

            if (!_context.Players.Any(player => player.Id == input.PlayerId))
                return BadRequest("A megadott játékos nem létezik.");

            if (!_context.Games.Any(game => game.Id == input.GameId))
                return BadRequest("A megadott játék nem létezik.");

            var review = new Rewiev
            {
                PlayerId = input.PlayerId,
                GameId = input.GameId,
                Points = input.Points,
                Comment = Clean(input.Comment)
                // A dátumot az adatbázis CURRENT_TIMESTAMP alapértéke állítja be.
            };

            try
            {
                _context.Reviews.Add(review);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Értékelés mentési hiba: {ex}");
                return StatusCode(500, "Nem sikerült menteni az értékelést.");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Rewiev input)
        {
            var validation = Validate(input);
            if (validation is not null)
                return BadRequest(validation);

            var review = _context.Reviews.FirstOrDefault(item => item.Id == id);
            if (review is null)
                return NotFound("Az értékelés nem található.");

            if (!_context.Players.Any(player => player.Id == input.PlayerId))
                return BadRequest("A megadott játékos nem létezik.");

            if (!_context.Games.Any(game => game.Id == input.GameId))
                return BadRequest("A megadott játék nem létezik.");

            review.PlayerId = input.PlayerId;
            review.GameId = input.GameId;
            review.Points = input.Points;
            review.Comment = Clean(input.Comment);

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review is null)
                return NotFound("Az értékelés nem található.");

            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return NoContent();
        }

        // Saját függvény 1: egy játék átlagpontszáma és értékeléseinek száma.
        [HttpGet("average/{gameId:int}")]
        public IActionResult GetAverageForGame(int gameId)
        {
            if (!_context.Games.Any(game => game.Id == gameId))
                return NotFound("A játék nem található.");

            var gameReviews = _context.Reviews.Where(review => review.GameId == gameId);
            int reviewCount = gameReviews.Count();
            double? averagePoints = gameReviews.Average(review => (double?)review.Points);

            return Ok(new
            {
                GameId = gameId,
                ReviewCount = reviewCount,
                AveragePoints = averagePoints
            });
        }

        // Saját függvény 2: egy játékos értékelései a játék nevével együtt.
        [HttpGet("player/{playerId:int}")]
        public IActionResult GetReviewsByPlayer(int playerId)
        {
            if (!_context.Players.Any(player => player.Id == playerId))
                return NotFound("A játékos nem található.");

            var playerReviews = _context.Reviews
                .Where(review => review.PlayerId == playerId)
                .Join(_context.Games,
                    review => review.GameId,
                    game => game.Id,
                    (review, game) => new
                    {
                        review.Id,
                        review.PlayerId,
                        review.GameId,
                        GameName = game.Name,
                        review.Points,
                        review.Comment,
                        review.ReviewReleaseDate
                    })
                .OrderByDescending(item => item.ReviewReleaseDate)
                .ToList();

            return Ok(playerReviews);
        }

        // Statisztikai összesítés a főoldalhoz.
        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            int reviewCount = _context.Reviews.Count();
            double? averagePoints = _context.Reviews.Average(review => (double?)review.Points);
            int reviewedGames = _context.Reviews
                .Select(review => review.GameId)
                .Distinct()
                .Count();

            return Ok(new
            {
                ReviewCount = reviewCount,
                AveragePoints = averagePoints,
                ReviewedGames = reviewedGames
            });
        }

        private static string? Validate(Rewiev input)
        {
            if (input.PlayerId <= 0)
                return "A játékos kiválasztása kötelező.";
            if (input.GameId <= 0)
                return "A játék kiválasztása kötelező.";
            if (input.Points < 1 || input.Points > 5)
                return "A pontszám 1 és 5 közötti legyen.";

            return null;
        }

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
