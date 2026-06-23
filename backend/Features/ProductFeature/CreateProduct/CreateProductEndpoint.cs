using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductFeature.CreateProduct;

[ApiController]
[Route("api/products")]
[Tags("Products")]
public class CreateProductController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateProductResponse>>> CreateProduct(
        [FromBody] CreateProductCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
