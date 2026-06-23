using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.DeleteBrand;

[ApiController]
[Route("api/brands/{id:long}")]
[Tags("Brands")]
public class DeleteBrandController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBrand(long id)
    {
        var result = await sender.Send(new DeleteBrandCommand(id));
        return result.ToActionResult();
    }
}
