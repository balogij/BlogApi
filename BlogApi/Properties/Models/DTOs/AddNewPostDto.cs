namespace BlogApi.Properties.Models.DTOs
{
    public class AddNewPostDto
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public int blogId { get; set; }
    }
}
