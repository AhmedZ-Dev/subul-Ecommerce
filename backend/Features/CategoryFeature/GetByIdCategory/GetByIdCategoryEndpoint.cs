using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CategoryFeature.GetByIdCategory;

[ApiController]
[Route("api/categories/{id:long}")]
[Tags("Categories")]
public class GetByIdCategoryController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetByIdCategoryResponse>>> GetByIdCategory(long id)
    {
        var result = await sender.Send(new GetByIdCategoryQuery(id));
        return result.ToActionResult();
    }
}
