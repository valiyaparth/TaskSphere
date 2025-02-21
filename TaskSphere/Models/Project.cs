namespace TaskSphere.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string? ImageUrl { get; set; }


        //Foreign key
        public User CreatorId { get; set; }


        //navigation properties
        public User Creator { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<ProjectMember> Members { get; set; }
        public ICollection<ProjectTeam> Teams { get; set; }
    }
}