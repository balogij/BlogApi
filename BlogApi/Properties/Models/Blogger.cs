using System.Text.Json.Serialization;

namespace BlogApi.Properties.Models
{
    public class Blogger
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Age { get; set; }

        // Biztonsági okokból elrejthetjük a jelszót a válaszból
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        public DateTime? RegistrationTime { get; set; }
    }
}
