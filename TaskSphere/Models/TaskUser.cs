using TaskSphere.Enums;

namespace TaskSphere.Models
{
    public class TaskUser
    {
        //composite key
        public int TaskId { get; set; }
        public string UserId { get; set; }

        //navigation properties
        public Task Task { get; set; }
        public User User { get; set; }

        //Role in the task
        public Roles Role { get; set; }
    }
}
