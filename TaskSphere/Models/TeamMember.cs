using TaskSphere.Enums;

namespace TaskSphere.Models
{
    public class TeamMember
    {
        //composite key
        public int TeamId { get; set; }
        public int UserId { get; set; }

        //navigation properties
        public Team Team { get; set; }
        public User User { get; set; }

        //Role in the team
        public Roles Role { get; set; }

    }
}
