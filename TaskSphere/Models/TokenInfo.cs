using System.ComponentModel.DataAnnotations;

namespace TaskSphere.Models
{
    public class TokenInfo
    {
        public int Id {  get; set; }

        [Required]
        [MaxLength(55)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiredAt { get; set; }
    }
}
