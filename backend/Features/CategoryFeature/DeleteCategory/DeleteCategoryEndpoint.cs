using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CategoryFeature.DeleteCategory;

[ApiController]
[Route("api/categories/{id:long}")]
[Tags("Categories")]
public class DeleteCategoryController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(long id)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id));
        return result.ToActionResult();
    }
}
