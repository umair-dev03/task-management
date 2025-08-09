using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Services
{
    /// <summary>
    /// Service for authenticating users and generating JWT tokens.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly TaskManagementDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(TaskManagementDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        public async Task<LoginResponseDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            // Find user by email
            var user = await _dbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            // For demo: assume password is stored as plain text (never do this in production)
            if (user == null || password != user.Password) // Replace with real password check
                return null;

            // Map user to UserDto
            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };

            // Generate JWT token
            var token = GenerateJwtToken(user, userDto.Roles);

            return new LoginResponseDto
            {
                User = userDto,
                Token = token
            };
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user, including roles in claims.
        /// </summary>
        private string GenerateJwtToken(User user, List<string> roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            // Add role claims
            claims.AddRange(roles.Select(role => new Claim("role", role)));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
