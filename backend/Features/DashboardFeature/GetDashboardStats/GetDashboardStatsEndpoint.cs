using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.DashboardFeature.GetDashboardStats;

[ApiController]
[Route("api/dashboard")]
[Tags("Dashboard")]
public class GetDashboardStatsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetDashboardStatsResponse>>> GetDashboardStats()
    {
        var result = await sender.Send(new GetDashboardStatsQuery());
        return result.ToActionResult();
    }
}
