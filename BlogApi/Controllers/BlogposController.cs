using BlogApi.Properties.Models;
using BlogApi.Properties.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Xml.Linq;

namespace BlogApi.Controllers
{
    [Route("blogpost")]
    [ApiController]
    public class BlogposController : Controller
    {
        public string ConnectionString = "server=localhost;database=blog;uid=root;password=;";
        [HttpGet]
        public object GetAllPost()
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogpost`";

            var cmd = new MySqlCommand(sql, connector);

            var posts = new List<Blogpost>();
            object? result = null;

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var post = new Blogpost
                {
                    id = datareader.GetInt32("Id"),
                    title = datareader.GetString("Title"),
                    content = datareader.GetString("Content"),
                    postTime = datareader.GetDateTime("postTime"),
                    updateTime = datareader.GetDateTime("updateTime"),
                    blogId = datareader.GetInt32("blogId")
                };

                posts.Add(post);
            }
            if (posts.Count != 0)
            {
                result = new { message = "Sikeres lekérdezés", result = posts };
            }
            else
            {
                result = new { message = "Nincsenek postok.", result = posts };
            }

            connector.Close();
            return result;

        }

        [HttpGet("postByBloggerId")]
        public object GetPostByBloggerId(int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogpost` WHERE `blogId` = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var posts = new List<Blogpost>();
            object? result = null;

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var post = new Blogpost
                {
                    id = datareader.GetInt32("Id"),
                    title = datareader.GetString("Title"),
                    content = datareader.GetString("Content"),
                    postTime = datareader.GetDateTime("postTime"),
                    updateTime = datareader.GetDateTime("updateTime"),
                    blogId = datareader.GetInt32("blogId")
                };

                posts.Add(post);
            }
            if(posts.Count!=0)
            {
                result = new { message = "Sikeres lekérdezés", result = posts };
            }
            else
            {
                result = new { message = "Nincs ilyen Id.", result = posts };
            }

            connector.Close();
            return result;

        }

        [HttpPost]
        public object AddNewBlogger(AddNewPostDto addNewPostDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogpost`(`Title`, `Content`, `postTime`, `updateTime`, `blogId`) VALUES (@title,@content,@postTime,@updateTime,@blogId)";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", addNewPostDto.title);
            cmd.Parameters.AddWithValue("@content", addNewPostDto.content);
            cmd.Parameters.AddWithValue("@postTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@blogId", addNewPostDto.blogId);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "Sikeres felvétel.", result = addNewPostDto };
        }

        //INSERT INTO `blogpost`(`Title`, `Content`, `postTime`, `updateTime`, `blogId`) VALUES ('Teszt Post','Ez a 21-es ID-jű Blogger teszt postja',2022-02-11 11:11:11,2022-02-11 11:11:11,21)

        [HttpDelete]
        public object DeleteBlogpost([FromBody] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM `blogpost` WHERE
            id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connector.Close();
            return new { message = "Sikeres törlés.", result = "" };
        }

        [HttpPut]
        public object UpdatePostDto(int id, UpdatePostDto updatePostDto)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"UPDATE `blogpost` SET `Title`=@title,`Content`=@content,`updateTime`=@updateTime WHERE `blogpost`.`Id` = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@title", updatePostDto.title);
            cmd.Parameters.AddWithValue("@content", updatePostDto.content);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);

            cmd.ExecuteNonQuery();

            connector.Close();
            return new { message = "Sikeres frissítés.", result = updatePostDto };
        }

        /*
{
  "id": 2,
  "title": "Második Blogger",
  "content": "Módosított post.",
  "blogId": 2
}
         */
    }
}
