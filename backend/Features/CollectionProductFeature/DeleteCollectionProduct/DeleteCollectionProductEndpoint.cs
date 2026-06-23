using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionProductFeature.DeleteCollectionProduct;

[ApiController]
[Route("api/collections/{collectionId:long}/products/{id:long}")]
[Tags("Collection Products")]
public class DeleteCollectionProductController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCollectionProduct(long collectionId, long id)
    {
        var result = await sender.Send(new DeleteCollectionProductCommand(collectionId, id));
        return result.ToActionResult();
    }
}
