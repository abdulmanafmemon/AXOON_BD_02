using FirstInternshipTask.DTO;
using FirstInternshipTask.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace FirstInternshipTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserController(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("signup")]
        public IActionResult Signup(UserSignupDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest(new { error = "Email already exists" });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = "user" 
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return StatusCode(201, new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto dto)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { error = "Invalid email or password" });

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { error = "Invalid email or password" });


    var jwtKey = _config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing in configuration");
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                message = "Login successful",
                token = tokenString
            });
        }

   
        [HttpGet("profile")]
        [Authorize]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                message = "Authenticated successfully",
                userId,
                email,
                role
            });
        }
    }
}
