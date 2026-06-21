using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CategoryFeature.CreateCategory;

[ApiController]
[Route("api/categories")]
[Tags("Categories")]
public class CreateCategoryController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateCategoryResponse>>> CreateCategory(
        [FromBody] CreateCategoryCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
