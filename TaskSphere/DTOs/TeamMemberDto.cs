using TaskSphere.Enums;

namespace TaskSphere.DTOs
{
    public class TeamMemberDto
    {
        //public int TeamId { get; set; }
        public int UserId { get; set; }
        public Roles Role { get; set; }
    }

    //public class AddTeamMemberDto
    //{
    //    public int UserId { get; set; }
    //    public Roles Role { get; set; }
    //}
}
