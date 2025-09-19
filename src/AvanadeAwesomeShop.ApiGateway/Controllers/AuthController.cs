using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AvanadeAwesomeShop.ApiGateway.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            // Validação de input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("🚫 Login INVALID INPUT: Username or Password empty, IP={ClientIp}", clientIp);
                return BadRequest(new { message = "Username e Password são obrigatórios" });
            }

            // Validação de tamanho (prevenir DoS)
            if (request.Username.Length > 50 || request.Password.Length > 100)
            {
                _logger.LogWarning("🚫 Login INVALID LENGTH: Username or Password too long, IP={ClientIp}", clientIp);
                return BadRequest(new { message = "Username ou Password muito longos" });
            }
            
            try
            {
                // Validação simples de credenciais (em produção usar Identity ou similar)
                if (IsValidUser(request.Username, request.Password))
                {
                    var userRole = GetUserRole(request.Username);
                    var token = GenerateJwtToken(request.Username);
                    
                    _logger.LogInformation("🔑 Login SUCCESS: User={Username}, Role={Role}, IP={ClientIp}", 
                        request.Username, userRole, clientIp);
                    
                    return Ok(new LoginResponse 
                    { 
                        Token = token,
                        ExpiresIn = 3600, // 1 hora
                        TokenType = "Bearer",
                        UserRole = userRole
                    });
                }

                _logger.LogWarning("🚫 Login FAILED: User={Username}, IP={ClientIp}", 
                    request.Username, clientIp);
                
                return Unauthorized(new { message = "Credenciais inválidas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Login ERROR: User={Username}, IP={ClientIp}", 
                    request.Username, clientIp);
                
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
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
            // Validação simples para demonstração com hash básico
            // Em produção: validar contra banco de dados com hash de senha
            var validUsers = new Dictionary<string, string>
            {
                // Senhas com hash SHA256 simples (para demo)
                { "admin", HashPassword("admin123") },
                { "manager", HashPassword("manager123") },
                { "user", HashPassword("user123") },
            };

            return validUsers.ContainsKey(username) && 
                   validUsers[username] == HashPassword(password);
        }

        private string HashPassword(string password)
        {
            // Hash simples para demo (em produção usar bcrypt)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt123"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GetUserRole(string username)
        {
            // Definir roles baseadas no username
            var userRoles = new Dictionary<string, string>
            {
                { "admin", "Admin" },
                { "manager", "Manager" },
                { "user", "User" },
            };

            return userRoles.GetValueOrDefault(username, "User");
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
            var userRole = GetUserRole(username);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim("role", userRole),
                    new Claim("username", username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            
            // Log seguro (sem expor token completo)
            _logger.LogDebug("🔑 JWT token generated for user: {Username}, expires: {Expires}", 
                username, tokenDescriptor.Expires);
            
            return tokenString;
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
        public string UserRole { get; set; } = string.Empty;
    }
}
