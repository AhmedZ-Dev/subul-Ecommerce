using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.OrderFeature.ListOrderPaginated;

[ApiController]
[Route("api/orders")]
[Tags("Orders")]
public class ListOrderPaginatedController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListOrderPaginatedResponse>>> ListOrders(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null,
        [FromQuery] long? userId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? paymentStatus = null,
        [FromQuery] string? fulfillmentStatus = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
    {
        var query = new ListOrderPaginatedQuery(
            page,
            limit,
            search,
            userId,
            status,
            paymentStatus,
            fulfillmentStatus,
            sortBy,
            sortOrder);

        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
