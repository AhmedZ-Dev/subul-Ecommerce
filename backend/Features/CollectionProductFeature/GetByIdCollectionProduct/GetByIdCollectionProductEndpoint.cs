using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionProductFeature.GetByIdCollectionProduct;

[ApiController]
[Route("api/collections/{collectionId:long}/products/{id:long}")]
[Tags("Collection Products")]
public class GetByIdCollectionProductController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CollectionProductLinkResponse>>> GetByIdCollectionProduct(
        long collectionId,
        long id)
    {
        var result = await sender.Send(new GetByIdCollectionProductQuery(collectionId, id));
        return result.ToActionResult();
    }
}
