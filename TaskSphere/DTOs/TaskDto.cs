using TaskSphere.Enums;
using TaskSphere.Models;

namespace TaskSphere.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public int CreatorId { get; set; }
        public int TeamId { get; set; }
        public int ProjectId { get; set; }
        public List<TaskUserDto> Assignee { get; set; } = new();
    }

    public class GetTaskDto
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public int TeamId { get; set; }
        public int ProjectId { get; set; }
        public List<TaskUserDto> Assignee { get; set; } = new();

    }
    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public DateOnly DueDate { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public DateOnly DueDate { get; set; }
        public string? ImageUrl { get; set; }
    }
}
