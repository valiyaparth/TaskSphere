namespace TaskSphere.DTOs
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<GetTaskDto> Tasks { get; set; } = new();
        public List<TeamMemberDto> TeamMembers { get; set; } = new();
        public List<ProjectTeamDto> Projects { get; set; } = new();
    }

    public class CreateTeamDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateTeamDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
