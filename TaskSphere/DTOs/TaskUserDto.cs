namespace TaskSphere.DTOs
{
    public class TaskUserDto
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }

    public class AssignTaskUserDto
    {
        public int UserId { get; set; }
    }
}
