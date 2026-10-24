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

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                // 1. Módszer: Az EF Core beépített kapcsolat-ellenőrzése
                bool canConnect = _context.Database.CanConnect();

                if (canConnect)
                {
                    // Opcionálisan letesztelhetjük a saját ExecuteSql függvényedet is:
                    var result = _context.ExecuteSql("SELECT 1");

                    return Ok(new { Message = "Sikeres csatlakozás a MySQL adatbázishoz! 🎉" });
                }
                else
                {
                    return StatusCode(500, "A szerver fut, de nem sikerült csatlakozni az adatbázishoz.");
                }
            }
            catch (Exception ex)
            {
                // Ha hiba van (pl. rossz jelszó, port vagy le van kapcsolva a MySQL), itt kiderül
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}