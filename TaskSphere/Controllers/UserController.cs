using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskSphere.DTOs;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;
using TaskSphere.Services;

namespace TaskSphere.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly ITokenService _tokenService;

        public UserController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<UserController> logger, ITokenService tokenService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenService = tokenService;
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
        public async Task<ActionResult<User>> GetUserById(string id)
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

        //Add a new User (Sign Up)
        [HttpPost("signup")]
        public async Task<ActionResult<User>> AddUser([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(registerUserDto.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            User user = new ()
            {
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUserDto.Email,
                EmailConfirmed = true,
                ImageUrl = registerUserDto.ImageUrl
            };

            var success = await _unitOfWork.User.CreateUserAsync(user, registerUserDto.Password);
            if (!success.Succeeded)
            {
                var errors = success.Errors.Select(e => e.Description);
                _logger.LogError(
                    $"Failed to create user. Errors: {string.Join(", ", errors)}"
                );
                return BadRequest($"Failed to create user. Errors: {string.Join(", ", errors)}");
            }
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        //Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginUserDto.Username);
                if (user == null)
                {
                    return BadRequest("Invalid login attempt. Please Register yourself.");
                }

                bool isValidPassword = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
                if (!isValidPassword)
                {
                    return Unauthorized();
                }

                //Generate Claims
                List<Claim> authClaims =
                [
                    new (ClaimTypes.Name, user.UserName),
                    new (ClaimTypes.NameIdentifier, user.Id),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ];

                //Generate Token 
                var token = _tokenService.GenerateAccessToken(authClaims);
                string refreshToken = _tokenService.GenerateRefreshToken();

                var tokenInfo = await _unitOfWork.TokenInfo.GetAsync(t => t.Username == user.UserName);

                //when user logs in generate new tokenInfos if null else update those
                if (tokenInfo == null)
                {
                    var ti = new TokenInfo
                    {
                        Username = user.UserName,
                        RefreshToken = refreshToken,
                        ExpiredAt = DateTime.UtcNow.AddDays(7)
                    };
                    await _unitOfWork.TokenInfo.AddAsync(ti);
                }
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(7);
                }

                await _unitOfWork.SaveAsync();

                return Ok(new
                {
                    token,
                    refreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An internal error occurred.");
            }
        }

        //Refresh endpoint
        [HttpPost("token/refresh")]
        public async Task<IActionResult> Refresh(TokenModel tokenModel)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
                var username = principal.Identity.Name;

                var tokenInfo = await _unitOfWork.TokenInfo.GetAsync(t => t.Username == username);
                if (tokenInfo == null 
                    || tokenInfo.RefreshToken != tokenModel.RefreshToken 
                    || tokenInfo.ExpiredAt <= DateTime.UtcNow)
                {
                    return BadRequest("Please Login again.");
                } 

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenInfo.RefreshToken = newRefreshToken;
                await _unitOfWork.SaveAsync();

                return Ok(new
                {
                    token = newAccessToken,
                    refreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //Update a User
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(string id, [FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _unitOfWork.User.GetAsync(t => t.Id == id);
            if (existingUser == null) return NotFound();

            existingUser.Name = registerUserDto.Name;
            existingUser.Email = registerUserDto.Email;
            //existingUser.PasswordHash = registerUserDto.Password;
            existingUser.ImageUrl = registerUserDto.ImageUrl;

            _unitOfWork.User.Update(existingUser);
            await _unitOfWork.SaveAsync();

            return Ok(existingUser);
        }

        //Delete a User
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
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
