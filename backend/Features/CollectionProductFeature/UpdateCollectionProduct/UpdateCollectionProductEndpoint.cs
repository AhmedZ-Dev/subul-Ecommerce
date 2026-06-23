using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionProductFeature.UpdateCollectionProduct;

[ApiController]
[Route("api/collections/{collectionId:long}/products/{id:long}")]
[Tags("Collection Products")]
public class UpdateCollectionProductController(ISender sender) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<CollectionProductLinkResponse>>> UpdateCollectionProduct(
        long collectionId,
        long id,
        [FromBody] UpdateCollectionProductRequest request)
    {
        var command = new UpdateCollectionProductCommand(collectionId, id, request.SortOrder);
        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
