using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductFeature.GetByIdProduct;

[ApiController]
[Route("api/products/{id:long}")]
[Tags("Products")]
public class GetByIdProductController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetByIdProductResponse>>> GetByIdProduct(long id)
    {
        var result = await sender.Send(new GetByIdProductQuery(id));
        return result.ToActionResult();
    }
}
