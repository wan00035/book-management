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

        // ==========================================
        // API: User Login
        // ==========================================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Base on the provided username and password, check the database to find the user's role
            string role = GetUserRoleFromDatabase(request.Username, request.Password);

            if (role != null)
            {
                // If found, generate a JWT token with the role claim and return it to the client
                var token = GenerateJwtToken(request.Username, role);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }

        // ==========================================
        // API: User Registration
        // ==========================================
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password cannot be empty." });
            }

            // 1. Check if the username already exists
            if (IsUsernameTaken(request.Username))
            {
                return Conflict(new { message = "Username is already taken." }); 
            }

            // 2. Create the new user with a default role of 'User'
            CreateNewUser(request.Username, request.Password, "User");

            return Ok(new { message = "Registration successful! You can now log in." });
        }

        // ==========================================
        // Helper Methods (Database Operations)
        // ==========================================
        private string GetUserRoleFromDatabase(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Role FROM Users WHERE Username = @Username AND Password = @Password";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
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
            }
            return null; 
        }

        private bool IsUsernameTaken(string username)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        private void CreateNewUser(string username, string password, string role)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    command.ExecuteNonQuery();
                }
            }
        }

        // ==========================================
        // Helper Methods (JWT Generation)
        // ==========================================
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

    // ==========================================
    // Data Transfer Objects (DTOs)
    // ==========================================
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}