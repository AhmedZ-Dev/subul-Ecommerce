using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.CreateBrand;

[ApiController]
[Route("api/brands")]
[Tags("Brands")]
public class CreateBrandController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateBrandResponse>>> CreateBrand(
        [FromBody] CreateBrandCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
