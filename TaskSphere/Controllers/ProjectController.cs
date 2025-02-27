using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TaskSphere.DTOs;
using TaskSphere.Enums;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var project = await _unitOfWork.Project.GetAsync(p => p.Id == id,
                includeProperties: new Expression<Func<Project, object>>[]
                {
                    project => project.Creator,
                    project => project.Members,
                    project => project.Tasks,
                    project => project.Teams
                });
            if (project == null) return NotFound();

            var projectDto = new ProjectDto
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

            return Ok(projectDto);
        }

        //Add a new Project
        [HttpPost]
        public async Task<ActionResult<Project>> AddProject([FromBody] CreateProjectDto createProjectDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                ImageUrl = createProjectDto.ImageUrl,
                CreatorId = 1, //TODO: Get the current user id
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

            // Retrieve project members
            var members = await _unitOfWork.ProjectMember
                .GetProjectMembersAsync(project.Id);

            // Retrive project teams
            var teams = await _unitOfWork.ProjectTeam.GetProjectTeamAsync(project.Id);

            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                ImageUrl = project.ImageUrl,
                CreatorId = project.CreatorId,
                Members = members.Select(member => new ProjectMemberDto
                {
                    ProjectId = member.ProjectId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Teams = teams.Select(team => new ProjectTeamDto
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

            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, projectDto);
        }

        //Update a Project
        [HttpPut("{id}")]
        public async Task<ActionResult<Project>> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var project = await _unitOfWork.Project.GetAsync(t => t.Id == id);
            if (project == null) return NotFound();

                project.Name = updateProjectDto.Name;
                project.Description = updateProjectDto.Description;
                project.ImageUrl = updateProjectDto.ImageUrl;
                project.CreatorId = project.CreatorId;
                project.CreatedAt = project.CreatedAt;
                project.UpdatedAt = DateTime.Now;

            // Retrieve project members
            var members = await _unitOfWork.ProjectMember
                .GetProjectMembersAsync(project.Id);

            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                ImageUrl = project.ImageUrl,
                CreatorId = project.CreatorId,
                Members = members.Select(member => new ProjectMemberDto
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

            _unitOfWork.Project.Update(project);
            await _unitOfWork.SaveAsync();

            return Ok(projectDto);
        }

        //Delete a Project
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            var projectToBeDeleted = await _unitOfWork.Project.GetAsync(t => t.Id == id);

            if (projectToBeDeleted == null)
                return NotFound();

            _unitOfWork.Project.Delete(projectToBeDeleted);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        //Add a user to a project
        [HttpPost("add-projectmember/{projectId}/{userId}")]
        public async Task<ActionResult> AddUserToProject(int projectId, int userId)
        {
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
        public async Task<ActionResult> RemoveUserFromProject(int projectId, int userId)
        {
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
