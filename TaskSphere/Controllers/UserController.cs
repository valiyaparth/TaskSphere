using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TaskSphere.DTOs;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //GetAll
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _unitOfWork.User.GetAllAsync(
                includeProperties: new Expression<Func<User, object>>[]
                {
                    user => user.Tasks,
                    user => user.Teams,
                    user => user.Projects
                });

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                TaskIds = user.Tasks?.Select(tu => tu.TaskId).ToList(),
                TeamIds = user.Teams?.Select(tm => tm.TeamId).ToList(),
                ProjectIds = user.Projects?.Select(pm => pm.ProjectId).ToList()
            }).ToList();

            return Ok(userDtos);
        }

        //Get User By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _unitOfWork.User.GetAsync(t => t.Id == id, 
                includeProperties: new Expression<Func<User, object>>[]
                {
                    user => user.Tasks,
                    user => user.Teams,
                    user => user.Projects
                });

            if (user == null) 
                return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                TaskIds = user.Tasks?.Select(tu => tu.TaskId).ToList(),
                TeamIds = user.Teams?.Select(tm => tm.TeamId).ToList(),
                ProjectIds = user.Projects?.Select(pm => pm.ProjectId).ToList()
            };
            return Ok(userDto);
        }

        //Add a new User
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User
            {
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
                Password = registerUserDto.Password,
                ImageUrl = registerUserDto.ImageUrl
            };

            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        //Update a User
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _unitOfWork.User.GetAsync(t => t.Id == id);
            if (existingUser == null) return NotFound();

            existingUser.Name = registerUserDto.Name;
            existingUser.Email = registerUserDto.Email;
            existingUser.Password = registerUserDto.Password;
            existingUser.ImageUrl = registerUserDto.ImageUrl;

            _unitOfWork.User.Update(existingUser);
            await _unitOfWork.SaveAsync();

            return Ok(existingUser);
        }

        //Delete a User
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var userToBeDeleted = await _unitOfWork.User.GetAsync(t => t.Id == id);

            if (userToBeDeleted == null)
                return NotFound();

            _unitOfWork.User.Delete(userToBeDeleted);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
