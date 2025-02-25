    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using TaskSphere.DTOs;
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

        }
    }
