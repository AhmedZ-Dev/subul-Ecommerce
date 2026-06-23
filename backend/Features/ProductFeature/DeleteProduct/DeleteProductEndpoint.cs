using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductFeature.DeleteProduct;

[ApiController]
[Route("api/products/{id:long}")]
[Tags("Products")]
public class DeleteProductController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(long id)
    {
        var result = await sender.Send(new DeleteProductCommand(id));
        return result.ToActionResult();
    }
}
