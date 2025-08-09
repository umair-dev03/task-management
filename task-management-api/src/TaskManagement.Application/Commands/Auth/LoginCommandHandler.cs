using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Commands.Auth
{
    /// <summary>
    /// Handler for LoginCommand, authenticates user and generates JWT token.
    /// </summary>
    public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IRepository<User> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponseDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            // Find user by email, including roles
            var user = await _userRepository.FirstOrDefaultAsync(
                u => u.Email == command.Email,
                q => q.Include(u => u.Roles),
                cancellationToken);

            // For demo: assume password is stored as plain text (never do this in production)
            if (user == null || user.Password != command.Password) // Replace with real password check
                return Result<LoginResponseDto>.Failure("Invalid email or password.");

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

            var response = new LoginResponseDto
            {
                User = userDto,
                Token = token
            };

            return Result<LoginResponseDto>.Success(response);
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
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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
