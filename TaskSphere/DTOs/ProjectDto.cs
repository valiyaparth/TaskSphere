namespace TaskSphere.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public int CreatorId { get; set; }

        public List<ProjectMemberDto> Members { get; set; } = new();
        public List<ProjectTeamDto> Teams { get; set; } = new();
        public List<GetTaskDto> Tasks { get; set; } = new();
    }

    public class CreateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
