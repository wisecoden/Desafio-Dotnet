using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AvanadeAwesomeShop.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validação simples de credenciais (em produção usar Identity ou similar)
            if (IsValidUser(request.Username, request.Password))
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new LoginResponse 
                { 
                    Token = token,
                    ExpiresIn = 3600, // 1 hora
                    TokenType = "Bearer"
                });
            }

            return Unauthorized(new { message = "Credenciais inválidas" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Simulação de registro (em produção usar Identity)
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username e Password são obrigatórios" });
            }

            // Simular registro bem-sucedido
            var token = GenerateJwtToken(request.Username);
            return Ok(new LoginResponse 
            { 
                Token = token,
                ExpiresIn = 3600,
                TokenType = "Bearer"
            });
        }

        private bool IsValidUser(string username, string password)
        {
            // Validação simples para demonstração
            // Em produção: validar contra banco de dados com hash de senha
            var validUsers = new Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "manager", "manager123" },
                { "user", "user123" },
                { "test", "test123" },
            };

            return validUsers.ContainsKey(username) && validUsers[username] == password;
        }

        private string GetUserRole(string username)
        {
            // Definir roles baseadas no username
            var userRoles = new Dictionary<string, string>
            {
                { "admin", "Admin" },
                { "manager", "Manager" },
                { "user", "User" },
                { "test", "User" }
            };

            return userRoles.GetValueOrDefault(username, "User");
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
            var userRole = GetUserRole(username);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("username", username),
                new Claim("role", userRole)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
    }
}
