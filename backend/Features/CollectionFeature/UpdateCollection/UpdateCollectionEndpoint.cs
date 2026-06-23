using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionFeature.UpdateCollection;

[ApiController]
[Route("api/collections")]
[Tags("Collections")]
public class UpdateCollectionController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UpdateCollectionResponse>>> UpdateCollection(
        long id,
        [FromBody] UpdateCollectionRequest request)
    {
        var command = new UpdateCollectionCommand(
            id,
            request.NameEn,
            request.NameAr,
            request.DescriptionEn,
            request.DescriptionAr,
            request.ImageUrl,
            request.BannerUrl,
            request.CollectionType,
            request.IsActive,
            request.SortOrder,
            request.MetaTitle,
            request.MetaDescription,
            request.Products,
            request.Slug);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
