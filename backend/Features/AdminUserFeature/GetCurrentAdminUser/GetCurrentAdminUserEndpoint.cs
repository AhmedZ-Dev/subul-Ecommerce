using System.Security.Claims;
using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AdminUserFeature.GetCurrentAdminUser;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
public class GetCurrentAdminUserController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<GetCurrentAdminUserResponse>>> GetCurrentAdminUser()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var adminUserId))
        {
            return Unauthorized(new ApiResponse<GetCurrentAdminUserResponse>(
                false,
                default,
                "unauthorized"));
        }

        var result = await sender.Send(new GetCurrentAdminUserQuery(adminUserId));
        return result.ToActionResult();
    }
}
