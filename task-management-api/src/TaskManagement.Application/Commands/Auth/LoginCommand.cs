using TaskManagement.Application.Common;

namespace TaskManagement.Application.Commands.Auth
{
    /// <summary>
    /// Command for user login.
    /// </summary>
    public class LoginCommand : ICommand<Result<LoginResponseDto>>
    {
        public string Email { get; }
        public string Password { get; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
