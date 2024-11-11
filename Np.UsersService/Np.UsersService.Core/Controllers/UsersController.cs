using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Business.Commands.DeleteUser;
using Np.UsersService.Core.Business.Commands.LogUser;
using Np.UsersService.Core.Business.Commands.RegisterUser;
using Np.UsersService.Core.Business.Commands.UpdatePassword;
using Np.UsersService.Core.Business.Commands.UpdateUserData;
using Np.UsersService.Core.Business.Queries.GetUser;
using Np.UsersService.Core.Dtos.Users;
using Np.UsersService.Core.Exceptions;
using System.Security.Claims;

namespace Np.UsersService.Core.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("/register")]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RegisterUserCommand(createUserRequest.Username, createUserRequest.Email, createUserRequest.Password));

        if (result.IsFailed && result.Error.Equals(RegisterUserErrors.EmailExists))
        {
            return BadRequest(result.Error.Description);
        }

        if (result.IsFailed && result.Error.Equals(RegisterUserErrors.UsernameExists))
        {
            return BadRequest(result.Error.Description);
        }

        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }

        return NoContent();
    }


    [HttpPost("/login")]
    public async Task<IActionResult> LogUser([FromBody]UserCredentials userCredentials, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LogUserCommand(userCredentials.Username, userCredentials.Password), cancellationToken);

        if(result.IsFailed && result.Error.Equals(LogUserErrors.UserUnathorized))
        {
            return BadRequest(result.Error.Description);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUser(CancellationToken cancellationToken)
    {

        var userId = GetUserId();

        if (userId == null) 
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new GetUserQuery(userId));

        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateUserData([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new UpdateUserDataCommand(userId)
            { 
                Username = request.Username, 
                Email = request.Email
            }, 
            cancellationToken);

        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }
        return NoContent();
    }

    [Authorize]
    [HttpPut("me/password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest updatePasswordRequest, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new UpdatePasswordCommand(userId, updatePasswordRequest.OldPassword, updatePasswordRequest.NewPassword));
        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }
        return NoContent();
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> RemoveUser(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new DeleteUserCommand(userId));
        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }
        return NoContent();
    }

    private string? GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
