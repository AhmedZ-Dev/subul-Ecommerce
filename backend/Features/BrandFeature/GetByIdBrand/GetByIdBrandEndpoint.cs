using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.GetByIdBrand;

[ApiController]
[Route("api/brands/{id:long}")]
[Tags("Brands")]
public class GetByIdBrandController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetByIdBrandResponse>>> GetByIdBrand(long id)
    {
        var result = await sender.Send(new GetByIdBrandQuery(id));
        return result.ToActionResult();
    }
}
