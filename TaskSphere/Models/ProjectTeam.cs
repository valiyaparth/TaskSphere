namespace TaskSphere.Models
{
    public class ProjectTeam
    {
        //composite key
        public int ProjectId { get; set; }
        public int TeamId { get; set; }

        //navigation properties
        public Project Project { get; set; }
        public Team Team { get; set; }
    }
}
