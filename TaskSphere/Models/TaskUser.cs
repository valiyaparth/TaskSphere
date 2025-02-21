namespace TaskSphere.Models
{
    public class TaskUser
    {
        //composite key
        public int TaskId { get; set; }
        public int UserId { get; set; }

        //navigation properties
        public Task Task { get; set; }
        public User User { get; set; }
    }
}
