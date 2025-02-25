namespace TaskSphere.DTOs
{
    public class ProjectTeamDto
    {
        public int ProjectId { get; set; }
        public int TeamId { get; set; }
    }

    public class AddProjectTeamDto
    {
        public int TeamId { get; set; }
    }
}
