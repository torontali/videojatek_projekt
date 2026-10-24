using Microsoft.EntityFrameworkCore;
using System.Data;

namespace backend.Models
{
    public class Application : DbContext
    {
        // FONTOOS !!!!!!!!!!!!! appsettings.json-be kell beírni a defaultConnectionString-t, és a Program.cs-ben kell beállítani a connectionString-et, hogy a DbContext tudja használni az adatbázist.
        public Application(DbContextOptions<Application> options) : base(options)
        {

        }

        public DataTable ExecuteSql(string sql) { 
            
            var connection = this.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open) { 
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;

        }




    }
}
