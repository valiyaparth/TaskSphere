    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using TaskSphere.DTOs;
using TaskSphere.Enums;
using TaskSphere.Models;
    using TaskSphere.Repository.IRepository;

    namespace TaskSphere.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class TaskController : ControllerBase
        {
            private readonly IUnitOfWork _unitOfWork;

            public TaskController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            //GetAll
            [HttpGet]
            public async Task<ActionResult<List<Models.Task>>> GetAllTasks()
            {
                var tasks = await _unitOfWork.Task.GetAllAsync(
                    includeProperties: new Expression<Func<Models.Task, object>>[]
                                        { task => task.Creator,
                                            task => task.Assignee,
                                            task => task.Team,
                                            task => task.Project
                                        });

                if (tasks == null) return NotFound();

                var taskDtos = tasks.Select(task => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    DueDate = task.DueDate,
                    ImageUrl = task.ImageUrl,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId,
                    Assignee = task.Assignee.Select(assignee => new TaskUserDto
                    {
                        UserId = assignee.UserId,
                        TaskId = assignee.TaskId,
                        roles = assignee.Role
                    }).ToList()
                }).ToList();

                return Ok(taskDtos);
            }

            //Get Task By Id
            [HttpGet("{id}")]
            public async Task<ActionResult<Models.Task>> GetTaskById(int id)
            {
                var task = await _unitOfWork.Task.GetAsync(t => t.Id == id, 
                    includeProperties: new Expression<Func<Models.Task, object>>[]
                    {
                        task => task.Creator,
                        task => task.Assignee,
                        task => task.Team,
                        task => task.Project
                    });
                if (task == null) return NotFound();

                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    DueDate = task.DueDate,
                    ImageUrl = task.ImageUrl,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId,
                    Assignee = task.Assignee.Select(assignee => new TaskUserDto
                    {
                        UserId = assignee.UserId,
                        TaskId = assignee.TaskId,
                        roles = assignee.Role
                    }).ToList()
                };

                return Ok(task);
            }

            //Add a new Task
            [HttpPost("add-task/{projectId}/{teamId}")]
            public async Task<ActionResult<Models.Task>> AddTask(int projectId, int teamId, [FromBody] CreateTaskDto createTaskDto)
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var projectExists = await _unitOfWork.Project.GetAsync(p => p.Id == projectId);
                if (projectExists == null) return BadRequest();

                var teamExists = await _unitOfWork.Team.GetAsync(t => t.Id == teamId);
                if (teamExists == null) return BadRequest();

                var task = new Models.Task
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Status = createTaskDto.Status,
                    DueDate = createTaskDto.DueDate,
                    ImageUrl = createTaskDto.ImageUrl,
                    ProjectId = projectId, // Assign the ProjectId
                    CreatorId = 1, // TODO : get the current user id
                    TeamId = teamId
                };

                await _unitOfWork.Task.AddAsync(task);
                await _unitOfWork.SaveAsync();

                //making creator an admin of the task
                var taskUser = new TaskUser
                {
                    TaskId = task.Id,
                    UserId = task.CreatorId,
                    Role = Roles.Admin
                };

                await _unitOfWork.TaskUser.AssignUserToTaskAsync(taskUser);
                await _unitOfWork.SaveAsync();

            var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    DueDate = task.DueDate,
                    UpdatedAt = task.UpdatedAt,
                    ImageUrl = task.ImageUrl,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId,
                    Assignee = task.Assignee.Select(assignee => new TaskUserDto
                    {
                        UserId = assignee.UserId,
                        TaskId = assignee.TaskId,
                        roles = assignee.Role
                    }).ToList()
            };

                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, taskDto);
            }

            //Update a Task
            [HttpPut("{id}")]
            public async Task<ActionResult<Models.Task>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var task = await _unitOfWork.Task.GetAsync(t => t.Id == id);
                if (task == null) return NotFound();

                task.Title = updateTaskDto.Title;
                task.Description = updateTaskDto.Description;
                task.Status = updateTaskDto.Status;
                task.UpdatedAt = DateTime.Now;
                task.DueDate = updateTaskDto.DueDate;
                task.ImageUrl = updateTaskDto.ImageUrl;

                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    DueDate = task.DueDate,
                    UpdatedAt = task.UpdatedAt,
                    ImageUrl = task.ImageUrl,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId,
                    Assignee = task.Assignee.Select(assignee => new TaskUserDto
                    {
                        UserId = assignee.UserId,
                        TaskId = assignee.TaskId,
                        roles = assignee.Role
                    }).ToList()
                };

                _unitOfWork.Task.Update(task);
                await _unitOfWork.SaveAsync();

                return Ok(taskDto);
            }

            //Delete a Task
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteTask(int id)
            {
                var taskToBeDeleted = await _unitOfWork.Task.GetAsync(t => t.Id == id);

                if (taskToBeDeleted == null)
                    return NotFound();

                _unitOfWork.Task.Delete(taskToBeDeleted);
                await _unitOfWork.SaveAsync();
                return NoContent();
            }

            //Add a assignee to a team
            [HttpPost("assign-task/{taskId}/{userId}")]
            public async Task<ActionResult> AssignTask(int taskId, int userId)
            {
                var task = await _unitOfWork.Task.GetAsync(t => t.Id == taskId);
                if (task == null) return NotFound();

                var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);
                if (user == null) return NotFound();

                var taskUser = new TaskUser
                {
                    TaskId = taskId,
                    UserId = userId,
                    Role = Roles.Member
                };
                
                await _unitOfWork.TaskUser.AssignUserToTaskAsync(taskUser);
                await _unitOfWork.SaveAsync();

                return Created();
            }

            //Remove a assignee from a task
            [HttpDelete("remove-assignee/{taskId}/{userId}")]
            public async Task<ActionResult> RemoveAssignee(int taskId, int userId)
            {
                var task = await _unitOfWork.Task.GetAsync(t => t.Id == taskId);
                if (task == null) return NotFound("Task Not Found");

                var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);
                if (user == null) return NotFound("User Not Found");

                TaskUser? taskUser = await _unitOfWork.TaskUser.GetAsync(tu => tu.TaskId == taskId && tu.UserId == userId);
                if (taskUser == null) return NotFound();

                await _unitOfWork.TaskUser.RemoveUserFromTaskAsync(taskUser);
                await _unitOfWork.SaveAsync();

            return NoContent();
            }
        }
    }
