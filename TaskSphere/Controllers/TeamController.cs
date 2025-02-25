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
    public class TeamController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeamController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //GetAll
        [HttpGet]
        public async Task<ActionResult<List<Team>>> GetAllTeams()
        {
            var teams = await _unitOfWork.Team.GetAllAsync(
                includeProperties: new Expression<Func<Team, object>>[]
                {
                    team => team.Creator,
                    team => team.Tasks,
                    team => team.TeamMembers,
                    team => team.Projects
                });

            var teamDtos = teams.Select(teams => new TeamDto
            {
                Id = teams.Id,
                Name = teams.Name,
                Description = teams.Description,
                ImageUrl = teams.ImageUrl,
                CreatorId = teams.CreatorId,
                CreatedAt = teams.CreatedAt,
                UpdatedAt = teams.UpdatedAt,
                Tasks = teams.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId

                }).ToList(),
                TeamMembers = teams.TeamMembers.Select(member => new TeamMemberDto
                {
                    //TeamId = member.TeamId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Projects = teams.Projects.Select(project => new ProjectTeamDto
                {
                    ProjectId = project.ProjectId,
                    TeamId = project.TeamId
                }).ToList()
            }).ToList();

            return Ok(teamDtos);
        }

        //Get Team by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeamById(int id)
        {
            var team = await _unitOfWork.Team.GetAsync(p => p.Id == id,
                includeProperties: new Expression<Func<Team, object>>[]
                {
                    team => team.Creator,
                    teams => teams.Tasks,
                    team => team.TeamMembers,
                    team => team.Projects
                });
            if (team == null) return NotFound();

            var teamDto = new TeamDto
            {
                Id = id,
                Name = team.Name,
                Description = team.Description,
                ImageUrl = team.ImageUrl,
                CreatorId = team.CreatorId,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt,
                Tasks = team.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId

                }).ToList(),
                TeamMembers = team.TeamMembers.Select(member => new TeamMemberDto
                {
                    //TeamId = member.TeamId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Projects = team.Projects.Select(Team => new ProjectTeamDto
                {
                    ProjectId = Team.ProjectId,
                    TeamId = Team.TeamId
                }).ToList()
            };

            return Ok(teamDto);
        }

        //Add a new Team
        [HttpPost]
        public async Task<ActionResult<Team>> AddTeam([FromBody] CreateTeamDto createTeamDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var team = new Team
            {
                Name = createTeamDto.Name,
                Description = createTeamDto.Description,
                ImageUrl = createTeamDto.ImageUrl,
                CreatorId = 1 //To do: get the current user id
            };

            await _unitOfWork.Team.AddAsync(team);
            await _unitOfWork.SaveAsync();

            //making creator an admin of the team
            var teamMember = new TeamMember
            {
                TeamId = team.Id,
                UserId = team.CreatorId,
                Role = Roles.Admin
            };

            //get members of the team
            var teamMembers = await _unitOfWork.TeamMember.GetTeamMembersAsync(team.Id);

            var teamDto = new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                ImageUrl = team.ImageUrl,
                CreatorId = team.CreatorId,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt,
                Tasks = team.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId
                }).ToList(),
                TeamMembers = teamMembers.Select(member => new TeamMemberDto
                {
                    //TeamId = member.TeamId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Projects = team.Projects.Select(project => new ProjectTeamDto
                {
                    ProjectId = project.ProjectId,
                    TeamId = project.TeamId
                }).ToList()
            };

            await _unitOfWork.TeamMember.AddUserToTeamAsync(teamMember);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, teamDto);
        }

        //Update a team
        [HttpPut("{id}")]
        public async Task<ActionResult<Team>> UpdateTeam(int id, [FromBody] UpdateTeamDto updateTeamDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var team = await _unitOfWork.Team.GetAsync(t => t.Id == id);
            if (team == null) return NotFound();

                team.Name = updateTeamDto.Name;
                team.Description = updateTeamDto.Description;
                team.ImageUrl = updateTeamDto.ImageUrl;
                team.CreatorId = team.CreatorId;
                team.CreatedAt = team.CreatedAt;
                team.UpdatedAt = DateTime.Now;

            //get members of the team
            var teamMembers = await _unitOfWork.TeamMember.GetTeamMembersAsync(team.Id);

            var teamDto = new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                ImageUrl = team.ImageUrl,
                CreatorId = team.CreatorId,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt,
                Tasks = team.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId
                }).ToList(),
                TeamMembers = teamMembers.Select(member => new TeamMemberDto
                {
                    //TeamId = member.TeamId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Projects = team.Projects.Select(Team => new ProjectTeamDto
                {
                    ProjectId = Team.ProjectId,
                    TeamId = Team.TeamId
                }).ToList()
            };


            _unitOfWork.Team.Update(team);
            await _unitOfWork.SaveAsync();

            return Ok(teamDto);
        }

        //Delete a team
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeam(int id)
        {
            var teamToBeDeleted = await _unitOfWork.Team.GetAsync(t => t.Id == id);

            if (teamToBeDeleted == null)
                return NotFound();

            _unitOfWork.Team.Delete(teamToBeDeleted);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        //Add user to a team
        [HttpPost("add-teammember/{teamId}/{userId}")]
        public async Task<ActionResult> AddUserToTeam(int teamId, int userId)
        {
            var team = await _unitOfWork.Team.GetAsync(t => t.Id == teamId);
            if (team == null)
                return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var teamMember = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                Role = Roles.Member
            };

            await _unitOfWork.TeamMember.AddUserToTeamAsync(teamMember);
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        //Remove user from a team
        [HttpDelete("remove-teammember/{teamId}/{userId}")]
        public async Task<ActionResult> RemoverUserFromTeam(int teamId, int userId)
        {
            var team = await _unitOfWork.Team.GetAsync(t => t.Id == teamId);
            if(team == null)
            {
                return NotFound();
            }

            TeamMember? teamMember = await _unitOfWork.TeamMember.GetAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
            if (teamMember == null)
            {
                return NotFound();
            }

            await _unitOfWork.TeamMember.RemoveUserFromTeamAsync(teamMember);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        //Add a task (this task will be added in this team only member of this team can view this task)
        //[HttpPost("add-task/{teamId}/{taskId}")]

    }
}
