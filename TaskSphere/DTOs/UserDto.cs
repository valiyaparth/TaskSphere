using System.ComponentModel.DataAnnotations;

namespace TaskSphere.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public List<int> TaskIds { get; set; }
        public List<int> TeamIds { get; set; }
        public List<int> ProjectIds { get; set; }
    }

    public class RegisterUserDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class LoginUserDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required] 
        public string Password { get; set; } = string.Empty;
    }
}
