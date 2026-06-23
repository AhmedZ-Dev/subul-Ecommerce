using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionProductFeature.CreateCollectionProduct;

[ApiController]
[Route("api/collections/{collectionId:long}/products")]
[Tags("Collection Products")]
public class CreateCollectionProductController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CollectionProductLinkResponse>>> CreateCollectionProduct(
        long collectionId,
        [FromBody] CreateCollectionProductRequest request)
    {
        var command = new CreateCollectionProductCommand(collectionId, request.ProductId, request.SortOrder);
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
