namespace TaskManagement.Api.Models
{
    /// <summary>
    /// DTO for login request.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// User's email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// User's password.
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
