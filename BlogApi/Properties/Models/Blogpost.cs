using System.Text.Json.Serialization;

namespace BlogApi.Properties.Models
{
    public class Blogpost
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public DateTime? postTime { get; set; }
        public DateTime? updateTime { get; set; }
        public int blogId { get; set; }
    }
}
