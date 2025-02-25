using TaskSphere.Enums;

namespace TaskSphere.DTOs
{
    public class ProjectMemberDto
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public Roles Role { get; set; }
    }

    public class AddProjectMemberDto
    {
        public int UserId { get; set; }
        public Roles Role { get; set; }
    }
}
