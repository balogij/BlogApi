using BlogApi.Properties.Models;
using BlogApi.Properties.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Xml.Linq;


namespace BlogApi.Controllers
{
    [Route("blogger")]
    [ApiController]
    public class BloggerController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=blog;uid=root;password=;";

        [HttpGet]
        public object GetAllBlogger()
        {
            var bloggers = new List<Blogger>();

            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = "SELECT * FROM blogger;";

            var cmd = new MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5)
                };

                bloggers.Add(blogger);
            }

            connector.Close();


            return new
            {
                message = "Sikeres lekérdezés",
                result = bloggers
            };
        }

        [HttpGet("byId")]
        public object GetBloggerById(int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogger` WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            Blogger? blogger = null;
            object? result = null;

            if (datareader.Read() == true)
            {
                blogger = new Blogger
                {
                    Id = datareader.GetInt32("id"),
                    Name = datareader.GetString("name"),
                    Email = datareader.GetString("email"),
                    Age = datareader.GetInt32("age"),
                    Password = datareader.GetString("password"),
                    RegistrationTime = datareader.GetDateTime("registrationTime")
                };

                result = new { message = "Sikeres lekérdezés", result = blogger };
            }
            else
            {
                result = new { message = "Nincs ilyen Id.", result = blogger };
            }

            connector.Close();
            return result;

        }

        [HttpGet("NameAndEmailbyId")]
        public object GetBloggerNameAndEmailById(int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogger` WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            BloggerNameAndEmail? blogger = null;
            object? result = null;

            if (datareader.Read() == true)
            {
                blogger = new BloggerNameAndEmail
                {
                    Name = datareader.GetString("name"),
                    Email = datareader.GetString("email"),
                };

                result = new { message = "Sikeres lekérdezés", result = blogger };
            }
            else
            {
                result = new { message = "Nincs ilyen Id.", result = blogger };
            }

            connector.Close();
            return result;

        }


        [HttpPost]
        public object AddNewBlogger(AddNewBloggerDto addNewBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogger`(`name`, `email`, `age`, `password`, `registrationTime`) 
                VALUES (@name,@email,@age,@password,@registrationTime)";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@name", addNewBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", addNewBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", addNewBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", addNewBloggerDto.Password);
            cmd.Parameters.AddWithValue("@registrationTime", DateTime.Now);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "Sikeres felvétel.", result = addNewBloggerDto };
        }

        [HttpDelete]
        public object DeleteBlogger([FromBody] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM `blogger` WHERE
            id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connector.Close();
            return new { message = "Sikeres törlés.", result = "" };
        }

        [HttpPut]
        public object UpdateBloggerDto(int id, UpdateBloggerDto updateBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"UPDATE `blogger` SET `name`=@name,`email`=@email,`age`=@age,`password`=@password WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@name", updateBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", updateBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", updateBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", updateBloggerDto.Password);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connector.Close();
            return new { message = "Sikeres frissítés.", result = updateBloggerDto };
        }

    }
}
/*
namespace BlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : Controller
    {
        private readonly MySqlConnection _connection;
        public BlogController(MySqlConnection connection)
        {
            _connection = connection;
        }

        // GET: api/Bloggers
        // Összes blogger lekérése
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blogger>>> GetBloggers()
        {
            var bloggers = new List<Blogger>();

            await _connection.OpenAsync();

            using var command = new MySqlCommand("SELECT id, name, email, age, RegistrationTime FROM blogger;", _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bloggers.Add(new Blogger
                {
                    Id = reader.GetInt32("id"),
    
                    Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString("name"),
    
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
    
                    Age = reader.IsDBNull(reader.GetOrdinal("age")) ? null : reader.GetInt32("age"),
    
                    RegistrationTime = reader.IsDBNull(reader.GetOrdinal("RegistrationTime")) ? null : reader.GetDateTime("RegistrationTime")
                });
            }

            return Ok(bloggers);
        }
    }
}
*/