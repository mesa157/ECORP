using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Data;
using UserService.DTOs;
using UserService.Model;
using Microsoft.EntityFrameworkCore;
using userService.DTOs;

namespace UserService.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(UserDbContext context, IConfiguration configuration, ILogger<UserController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // Register User
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (_context.Users.Any(u => u.Email == registerDto.Email))
                {
                    _logger.LogWarning($"Email {registerDto.Email} is already taken.");
                    return BadRequest("Email is already taken.");
                }

                if (!IsPasswordStrong(registerDto.Password))
                {
                    _logger.LogWarning("Weak password provided during registration.");
                    return BadRequest("Password is not strong enough. It must contain at least 8 characters, including an uppercase letter, a number, and a special character.");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    PasswordHash = hashedPassword,
                    IsActive = true,
                    Role = "User" // Default role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {user.Email} registered successfully with ID: {user.UserId}.");
                return Ok(new { UserId = user.UserId, Message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Login User
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning("Login attempt with null or empty email/password.");
                    return BadRequest("Email and password must be provided.");
                }

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"Failed login attempt for email: {loginDto.Email}");
                    return Unauthorized("Invalid credentials.");
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation($"User {user.Email} logged in successfully with ID: {user.UserId}.");
                return Ok(new { Token = token, UserId = user.UserId }); // Return UserId along with the token
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login attempt: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Get User Profile
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return NotFound("User not found.");
                }

                var userProfileDto = new UserProfileDto
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };

                return Ok(userProfileDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user profile for ID {userId}");
                return StatusCode(500, "Internal server error");
            }
        }


        // Update User Profile
        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUserProfile(Guid userId, [FromBody] UserProfileDto userProfileDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return NotFound("User not found.");
                }

                user.Name = userProfileDto.Name ?? user.Name;
                user.Email = userProfileDto.Email ?? user.Email;
                user.Phone = userProfileDto.Phone ?? user.Phone;
                user.Address = userProfileDto.Address ?? user.Address;
                user.ProfilePictureUrl = userProfileDto.ProfilePictureUrl ?? user.ProfilePictureUrl;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User profile for {userId} updated successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user profile: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Delete User
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return NotFound("User not found.");
                }

                user.IsActive = false; // Mark user as inactive
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User with ID {userId} deactivated successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating user: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool IsPasswordStrong(string password)
        {
            return password.Length >= 8
                && password.Any(char.IsDigit)
                && password.Any(char.IsUpper)
                && password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
