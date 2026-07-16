using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AdminUserFeature.LogoutAdminUser;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
public class LogoutAdminUserController(ISender sender) : ControllerBase
{
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<LogoutAdminUserResponse>>> LogoutAdminUser()
    {
        var result = await sender.Send(new LogoutAdminUserCommand());
        return result.ToActionResult();
    }
}
