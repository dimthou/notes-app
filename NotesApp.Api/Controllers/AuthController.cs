using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NotesApp.Api.Data;
using Dapper;
using NotesApp.Api.DTOs;

namespace NotesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DapperContext _context;

        public AuthController(IConfiguration config, DapperContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterDto dto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            using var conn = _context.CreateConnection();
            var sql = @"INSERT INTO Users (Username, Email, PasswordHash)
                        VALUES (@Username, @Email, @PasswordHash);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            var userId = await conn.ExecuteScalarAsync<int>(sql, new { dto.Username, dto.Email, PasswordHash = passwordHash });
            return Ok(new { userId, dto.Username, dto.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto dto)
        {
            using var conn = _context.CreateConnection();
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            var user = await conn.QuerySingleOrDefaultAsync<dynamic>(sql, new { dto.Username });

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, (string)user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken((int)user.Id, (string)user.Username);
            return Ok(new { token });
        }

        private string GenerateJwtToken(int userId, string username)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
