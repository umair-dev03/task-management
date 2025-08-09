namespace TaskManagement.Application.Common
{
    /// <summary>
    /// Authentication service interface for user login and JWT generation.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's password.</param>
        /// <returns>LoginResponseDto containing user data and JWT token, or null if authentication fails.</returns>
        Task<LoginResponseDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
