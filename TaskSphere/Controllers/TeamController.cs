using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskSphere.DTOs;
using TaskSphere.Enums;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public TeamController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
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

            if(teams == null) return NotFound();

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

            //check whether user exists
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found.");
            }

            //creating new team
            var team = new Team
            {
                Name = createTeamDto.Name,
                Description = createTeamDto.Description,
                ImageUrl = createTeamDto.ImageUrl,
                CreatorId = userId
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

            //and adding creator to the team
            await _unitOfWork.TeamMember.AddUserToTeamAsync(teamMember);
            await _unitOfWork.SaveAsync();

            //eager loading 
            var teamDetails = await _unitOfWork.Team.GetAsync(p => p.Id == team.Id,
                includeProperties: new Expression<Func<Team, object>>[]
                {
                    team => team.Creator,
                    teams => teams.Tasks,
                    team => team.TeamMembers,
                    team => team.Projects
                });
            if (teamDetails == null) return NotFound();

            //creating team dto
            var teamDto = new TeamDto
            {
                Id = teamDetails.Id,
                Name = teamDetails.Name,
                Description = teamDetails.Description,
                ImageUrl = teamDetails.ImageUrl,
                CreatorId = teamDetails.CreatorId,
                CreatedAt = teamDetails.CreatedAt,
                UpdatedAt = teamDetails.UpdatedAt,
                Tasks = teamDetails.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId
                }).ToList(),
                TeamMembers = teamDetails.TeamMembers.Select(member => new TeamMemberDto
                {
                    TeamId = member.TeamId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Projects = teamDetails.Projects.Select(project => new ProjectTeamDto
                {
                    ProjectId = project.ProjectId,
                    TeamId = project.TeamId
                }).ToList()
            };

            return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, teamDto);
        }

        //Update a team
        [HttpPut("{teamId}")]
        public async Task<ActionResult<Team>> UpdateTeam(int teamId, [FromBody] UpdateTeamDto updateTeamDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //authorization
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            //get team
            var team = await _unitOfWork.Team.GetAsync(t => t.Id == teamId);
            if (team == null) return NotFound();

            //update team
                team.Name = updateTeamDto.Name;
                team.Description = updateTeamDto.Description;
                team.ImageUrl = updateTeamDto.ImageUrl;
                team.CreatorId = team.CreatorId;
                team.CreatedAt = team.CreatedAt;
                team.UpdatedAt = DateTime.Now;

            //eager loading
            var teamDetails = await _unitOfWork.Team.GetAsync(p => p.Id == team.Id,
                includeProperties: new Expression<Func<Team, object>>[]
                {
                    team => team.Creator,
                    teams => teams.Tasks,
                    team => team.TeamMembers,
                    team => team.Projects
                });
            if (teamDetails == null) return NotFound();

            //creating team dto
            var teamDto = new TeamDto
            {
                Id = teamDetails.Id,
                Name = teamDetails.Name,
                Description = teamDetails.Description,
                ImageUrl = teamDetails.ImageUrl,
                CreatorId = teamDetails.CreatorId,
                CreatedAt = teamDetails.CreatedAt,
                UpdatedAt = teamDetails.UpdatedAt,
                Tasks = teamDetails.Tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    CreatorId = task.CreatorId,
                    TeamId = task.TeamId,
                    ProjectId = task.ProjectId
                }).ToList(),
                TeamMembers = teamDetails.TeamMembers.Select(member => new TeamMemberDto
                {
                    TeamId = member.TeamId,
                    UserId = member.UserId,
                    Role = member.Role
                }).ToList(),
                Projects = teamDetails.Projects.Select(project => new ProjectTeamDto
                {
                    ProjectId = project.ProjectId,
                    TeamId = project.TeamId
                }).ToList()
            };


            _unitOfWork.Team.Update(team);
            await _unitOfWork.SaveAsync();

            return Ok(teamDto);
        }

        //Delete a team
        [HttpDelete("{teamId}")]
        public async Task<ActionResult> DeleteTeam(int teamId)
        {

            //authorization
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            //check if team exists
            var teamToBeDeleted = await _unitOfWork.Team.GetAsync(t => t.Id == teamId);
            if (teamToBeDeleted == null)
                return NotFound();

            _unitOfWork.Team.Delete(teamToBeDeleted);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        //Add user to a team
        [HttpPost("add-teammember/{teamId}/{userId}")]
        public async Task<ActionResult> AddUserToTeam(int teamId, string userId)
        {
            //authorization
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if(!authorization.Succeeded)
            {
                return Forbid();
            }

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

            return Created();
        }

        //Remove user from a team
        [HttpDelete("remove-teammember/{teamId}/{userId}")]
        public async Task<ActionResult> RemoverUserFromTeam(int teamId, string userId)
        {
            //authorization
            var authorization = await _authorizationService.AuthorizeAsync(User, null, "Admin");
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

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
    }
}
