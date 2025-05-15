using Microsoft.AspNetCore.Identity;

namespace TaskSphere.Models
{
    public class User : IdentityUser
    {
        //public int Id { get; set; }
        public string Name { get; set; }
        
        //public string Email { get; set; }
        //public string Password { get; set; }
        
        public string? ImageUrl { get; set; }

        //navigation properties
        public ICollection<TaskUser>? Tasks { get; set; }
        public ICollection<TeamMember>? Teams { get; set; }
        public ICollection<ProjectMember> Projects { get; set; }
    }
}
