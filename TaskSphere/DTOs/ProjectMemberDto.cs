using TaskSphere.Enums;

namespace TaskSphere.DTOs
{
    public class ProjectMemberDto
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public Roles Role { get; set; }
    }

    public class AddProjectMemberDto
    {
        public string UserId { get; set; }
        public Roles Role { get; set; }
    }
}
