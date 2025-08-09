using System.Collections.Generic;

namespace TaskManagement.Application.Common
{
    /// <summary>
    /// DTO for user data in application responses.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User's unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User's username.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// User's email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// List of user roles.
        /// </summary>
        public List<string> Roles { get; set; } = new();
    }
}
