using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AdminUserFeature.LoginAdminUser;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
public class LoginAdminUserController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginAdminUserResponse>>> LoginAdminUser(
        [FromBody] LoginAdminUserCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
