using Microsoft.AspNetCore.Mvc;
using backend.Models; // Szükséges az ApplicationDbContext miatt

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly Application _context;

        // Konstruktor, amivel a .NET beadja (injektálja) az adatbázis-kontextust
        public GamesController(Application context)
        {
            _context = context;
        }

        
    }
}