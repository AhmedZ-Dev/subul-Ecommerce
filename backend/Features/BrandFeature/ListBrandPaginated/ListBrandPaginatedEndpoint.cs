using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.ListBrandPaginated;

[ApiController]
[Route("api/brands")]
[Tags("Brands")]
public class ListBrandPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListBrandPaginatedResponse>>> ListBrands(
        [FromQuery] ListBrandPaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
