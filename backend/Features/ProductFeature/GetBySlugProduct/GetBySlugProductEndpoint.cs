using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.ProductFeature.GetByIdProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductFeature.GetBySlugProduct;

[ApiController]
[Route("api/products/by-slug")]
[Tags("Products")]
public class GetBySlugProductController(ISender sender) : ControllerBase
{
    /// <summary>Storefront catalog — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult<ApiResponse<GetByIdProductResponse>>> GetBySlugProduct(string slug)
    {
        var result = await sender.Send(new GetBySlugProductQuery(slug));
        return result.ToActionResult();
    }
}
