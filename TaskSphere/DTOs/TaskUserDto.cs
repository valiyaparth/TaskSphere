using TaskSphere.Enums;

namespace TaskSphere.DTOs
{
    public class TaskUserDto
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public Roles roles { get; set; }
    }

    public class AssignTaskToUserDto
    {
        public int UserId { get; set; }
        public Roles Role { get; set; }
    }
}
