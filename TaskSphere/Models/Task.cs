using System.Text.Json.Serialization;
using TaskSphere.Enums;

namespace TaskSphere.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ImageUrl { get; set; }


        //Foreign keys
        public string CreatorId { get; set; }
        public int TeamId { get; set; }
        public int ProjectId { get; set; }  


        //navigation property
        public User Creator { get; set; }
        public ICollection<TaskUser> Assignee { get; set; }
        public Team Team { get; set; }
        public Project Project { get; set; }

    }
}
