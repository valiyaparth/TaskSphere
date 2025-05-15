using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using TaskSphere.DTOs;
using TaskSphere.Enums;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDistributedCache _cache;

        public ProjectController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _cache = cache;
        }

        //GetAll
        [HttpGet]
        public async Task<ActionResult<List<Project>>> GetAllProjects()
        {
            var projects = await _unitOfWork.Project.GetAllAsync(
                includeProperties: new Expression<Func<Project, object>>[]
                {
                    project => project.Creator,
                    project => project.Members,
                    project => project.Tasks,
                    project => project.Teams
                });

            if(projects == null) return NotFound();


            var projectDtos = projects.Select(projects => new ProjectDto
            {
                Id = projects.Id,
                Name = projects.Name,
                Description = projects.Description,
                CreatedAt = projects.CreatedAt,
                UpdatedAt = projects.UpdatedAt,
                ImageUrl = projects.ImageUrl,
                CreatorId = projects.CreatorId,
                Members = projects.Members.Select(member => new ProjectMemberDto
                {
                    ProjectId = member.ProjectId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Teams = projects.Teams.Select(team => new ProjectTeamDto
                {
                    ProjectId = team.ProjectId,
                    TeamId = team.TeamId
                }).ToList(),
                Tasks = projects.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId
                }).ToList()
            }).ToList();
            return Ok(projectDtos);
        }

        //Get Project by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var cacheKey = $"Project_{id}";
            Project? project;
            ProjectDto? projectDto;
            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey);
                //cache hit
                if (!string.IsNullOrEmpty(cachedData))
                {
                    projectDto = JsonSerializer.Deserialize<ProjectDto>(cachedData);
                    if (projectDto == null)
                        _cache.Remove(cacheKey);
                }
                //cache miss
                else
                {
                    project = await _unitOfWork.Project.GetAsync(p => p.Id == id,
                        includeProperties: new Expression<Func<Project, object>>[]
                        {
                            project => project.Creator,
                            project => project.Members,
                            project => project.Tasks,
                            project => project.Teams
                        });
                    if (project == null) return NotFound();

                    projectDto = new ProjectDto
                    {
                        Id = project.Id,
                        Name = project.Name,
                        Description = project.Description,
                        CreatedAt = project.CreatedAt,
                        UpdatedAt = project.UpdatedAt,
                        ImageUrl = project.ImageUrl,
                        CreatorId = project.CreatorId,
                        Members = project.Members.Select(member => new ProjectMemberDto
                        {
                            ProjectId = member.ProjectId,
                            UserId = member.UserId,
                            Role = member.Role
                        }).ToList(),
                        Teams = project.Teams.Select(team => new ProjectTeamDto
                        {
                            ProjectId = team.ProjectId,
                            TeamId = team.TeamId
                        }).ToList(),
                        Tasks = project.Tasks.Select(task => new GetTaskDto
                        {
                            Id = task.Id,
                            CreatorId = task.CreatorId,
                            TeamId = task.TeamId,
                            ProjectId = task.ProjectId
                        }).ToList()
                    };

                    var serializedData = JsonSerializer.Serialize(projectDto);

                    await _cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    });
                }
                return Ok(projectDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while accessing the cache.", details = ex.Message });
            }
        }

        //Add a new Project
        [HttpPost]
        public async Task<ActionResult<Project>> AddProject([FromBody] CreateProjectDto createProjectDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                ImageUrl = createProjectDto.ImageUrl,
                CreatorId = userId
            };

            await _unitOfWork.Project.AddAsync(project);
            await _unitOfWork.SaveAsync();

            //making creator an admin of the project
            var projectMember = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = project.CreatorId,
                Role = Roles.Admin,
            };


            // and adding the creator to project member table
            await _unitOfWork.ProjectMember.AddUserToProjectAsync(projectMember);
            await _unitOfWork.SaveAsync();

            var projectDetails = await _unitOfWork.Project.GetAsync(p => p.Id == project.Id,
                includeProperties: new Expression<Func<Project, object>>[]
                {
                    project => project.Creator,
                    project => project.Members,
                    project => project.Tasks,
                    project => project.Teams
                });
            if (projectDetails == null) return NotFound();

            var projectDto = new ProjectDto
            {
                Id = projectDetails.Id,
                Name = projectDetails.Name,
                Description = projectDetails.Description,
                CreatedAt = projectDetails.CreatedAt,
                UpdatedAt = projectDetails.UpdatedAt,
                ImageUrl = projectDetails.ImageUrl,
                CreatorId = userId,
                Members = projectDetails.Members.Select(member => new ProjectMemberDto
                {
                    ProjectId = member.ProjectId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Teams = projectDetails.Teams.Select(team => new ProjectTeamDto
                {
                    ProjectId = team.ProjectId,
                    TeamId = team.TeamId
                }).ToList(),
                Tasks = projectDetails.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId
                }).ToList()
            };
            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, projectDto);
        }

        //Update a Project
        [HttpPut("{projectId}")]
        public async Task<ActionResult<Project>> UpdateProject(int projectId, [FromBody] UpdateProjectDto updateProjectDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //authorization
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if(!authorization.Succeeded)
            {
                return Forbid();
            }

            //get project
            var project = await _unitOfWork.Project.GetAsync(t => t.Id == projectId);
            if (project == null) return NotFound();

            //update project
                project.Name = updateProjectDto.Name;
                project.Description = updateProjectDto.Description;
                project.ImageUrl = updateProjectDto.ImageUrl;
                project.CreatorId = project.CreatorId;
                project.CreatedAt = project.CreatedAt;
                project.UpdatedAt = DateTime.Now;

            //eager loading
            var projectDetails = await _unitOfWork.Project.GetAsync(p => p.Id == project.Id,
                includeProperties: new Expression<Func<Project, object>>[]
                {
                    project => project.Creator,
                    project => project.Members,
                    project => project.Tasks,
                    project => project.Teams
                });
            if (projectDetails == null) return NotFound();

            //create project dto to return
            var projectDto = new ProjectDto
            {
                Id = projectDetails.Id,
                Name = projectDetails.Name,
                Description = projectDetails.Description,
                CreatedAt = projectDetails.CreatedAt,
                UpdatedAt = projectDetails.UpdatedAt,
                ImageUrl = projectDetails.ImageUrl,
                CreatorId = projectDetails.CreatorId,
                Members = projectDetails.Members.Select(member => new ProjectMemberDto
                {
                    ProjectId = member.ProjectId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Teams = projectDetails.Teams.Select(team => new ProjectTeamDto
                {
                    ProjectId = team.ProjectId,
                    TeamId = team.TeamId
                }).ToList(),
                Tasks = projectDetails.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId
                }).ToList()
            };

            _unitOfWork.Project.Update(project);
            await _unitOfWork.SaveAsync();

            return Ok(projectDto);
        }

        //Delete a Project
        [HttpDelete("{projectId}")]
        public async Task<ActionResult> DeleteProject(int projectId)
        {
            //authorization to check if user is admin of this project or not
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            //check if project exists
            var projectToBeDeleted = await _unitOfWork.Project.GetAsync(t => t.Id == projectId);
            if (projectToBeDeleted == null)
                return NotFound();

            _unitOfWork.Project.Delete(projectToBeDeleted);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }


        //Add a user to a project
        [HttpPost("add-projectmember/{projectId}/{userId}")]
        public async Task<ActionResult> AddUserToProject(int projectId, string userId)
        {
            //authorization to check if user is admin of this project or not
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var project = await _unitOfWork.Project.GetAsync(p => p.Id == projectId);
            if (project == null)
                return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId,
                Role = Roles.Member
            };

            await _unitOfWork.ProjectMember.AddUserToProjectAsync(projectMember);
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        //Remove a user from a project
        [HttpDelete("remove-projectmember/{projectId}/{userId}")]
        public async Task<ActionResult> RemoveUserFromProject(int projectId, string userId)
        {
            //authorization to check if user is admin of this project or not
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
            var project = await _unitOfWork.Project.GetAsync(p => p.Id == projectId);
            if (project == null)
                return NotFound();

            ProjectMember? projectMember = await _unitOfWork.ProjectMember.GetAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
            if (projectMember == null)
                return NotFound();

            await _unitOfWork.ProjectMember.RemoveUserFromProjectAsync(projectMember);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        //Add a team to project
        [HttpPost("add-team/{projectId}/{teamId}")]
        public async Task<ActionResult> AddTeamToProject(int projectId, int teamId)
        {
            //authorization to check if user is admin of this project or not
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
            var project = await _unitOfWork.Project.GetAsync(p => p.Id == projectId);
            if (project == null)
                return NotFound();
            var team = await _unitOfWork.Team.GetAsync(t => t.Id == teamId);
            if (team == null)
                return NotFound();
            var projectTeam = new ProjectTeam
            {
                ProjectId = projectId,
                TeamId = teamId
            };
            await _unitOfWork.ProjectTeam.AddTeamToProjectAsync(projectTeam);
            await _unitOfWork.SaveAsync();
            return Created();
        }

        //Remove a team from project
        [HttpDelete("remove-team/{projectId}/{teamId}")]
        public async Task<ActionResult> RemoveTeamFromProject(int projectId, int teamId)
        {
            //authorization to check if user is admin of this project or not
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
            var project = await _unitOfWork.Project.GetAsync(p => p.Id == projectId);
            if (project == null)
                return NotFound();
            var team = await _unitOfWork.Team.GetAsync(t => t.Id == teamId);
            if (team == null)
                return NotFound();

            ProjectTeam? projectTeam = await _unitOfWork.ProjectTeam.GetAsync(pt => pt.ProjectId == projectId && pt.TeamId == teamId);
            if (projectTeam == null)
                return NotFound();

            await _unitOfWork.ProjectTeam.RemoveTeamFromProjectAsync(projectTeam);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
