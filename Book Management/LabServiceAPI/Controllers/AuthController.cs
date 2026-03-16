using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AuthController(IConfiguration config)
        {
            _config = config;
            
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
            string user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
            string pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "root";
            string db = Environment.GetEnvironmentVariable("DB_NAME") ?? "Books";

            _connectionString = $"server={host};port={port};user={user};password={pass};database={db}";
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // base on the provided username and password, check the database to find the user's role
            string role = GetUserRoleFromDatabase(request.Username, request.Password);

            if (role != null)
            {
                // if found, generate a JWT token with the role claim and return it to the client
                var token = GenerateJwtToken(request.Username, role);
                return Ok(new { token });
            }

            return Unauthorized("Invalid username or password");
        }

        private string GetUserRoleFromDatabase(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                
                string query = "SELECT Role FROM Users WHERE Username = @Username AND Password = @Password";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString("Role"); 
                    }
                }
            }
            return null; 
        }

        private string GenerateJwtToken(string username, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}