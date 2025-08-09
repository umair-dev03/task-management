namespace TaskManagement.Application.Common
{
    /// <summary>
    /// DTO for login response, containing user data and JWT token.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// The authenticated user.
        /// </summary>
        public UserDto User { get; set; } = null!;

        /// <summary>
        /// The JWT token for authentication.
        /// </summary>
        public string Token { get; set; } = null!;
    }
}
