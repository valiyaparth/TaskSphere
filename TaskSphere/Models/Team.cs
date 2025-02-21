namespace TaskSphere.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }

        //Foreign key
        public int CreatorId { get; set; }

        //navigation properties
        public User Creator { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<TeamMember> TeamMembers { get; set; }
        public ICollection<ProjectTeam> Projects { get; set; }
    }
}
