using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Models;
using TaskManagement.Application.Common;
using TaskManagement.Application.Commands.Auth;

namespace TaskManagement.Api.Controllers
{
    /// <summary>
    /// Controller for authentication-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandler<LoginCommand, Result<TaskManagement.Application.Common.LoginResponseDto>> _loginHandler;

        public AuthController(ICommandHandler<LoginCommand, Result<TaskManagement.Application.Common.LoginResponseDto>> loginHandler)
        {
            _loginHandler = loginHandler;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token with user info and roles.
        /// </summary>
        /// <param name="loginRequest">Login request DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User info and JWT token if successful, 401 otherwise.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(loginRequest.Email, loginRequest.Password);
            var result = await _loginHandler.Handle(command, cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized();

            

            return Ok(result);
        }
    }
}
