namespace TaskSphere.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public List<int> TaskIds { get; set; }
        public List<int> TeamIds { get; set; }
        public List<int> ProjectIds { get; set; }
    }

    public class RegisterUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
