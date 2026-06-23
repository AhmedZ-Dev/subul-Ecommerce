using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionProductFeature.CreateCollectionProduct;

public class CreateCollectionProductHandler(AppDbContext context)
    : IRequestHandler<CreateCollectionProductCommand, Result<CollectionProductLinkResponse>>
{
    public async Task<Result<CollectionProductLinkResponse>> Handle(
        CreateCollectionProductCommand command,
        CancellationToken cancellationToken)
    {
        var collectionExists = await context.Collections.AnyAsync(
            c => c.Id == command.CollectionId,
            cancellationToken);

        if (!collectionExists)
            return Result<CollectionProductLinkResponse>.Failure("Collection not found");

        var product = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken);

        if (product is null)
            return Result<CollectionProductLinkResponse>.Failure("Product not found");

        var linkExists = await context.CollectionProducts.AnyAsync(
            cp => cp.CollectionId == command.CollectionId && cp.ProductId == command.ProductId,
            cancellationToken);

        if (linkExists)
            return Result<CollectionProductLinkResponse>.Failure("Collection product already exists");

        var link = new CollectionProduct
        {
            CollectionId = command.CollectionId,
            ProductId = command.ProductId,
            SortOrder = command.SortOrder,
            CreatedAt = DateTime.Now
        };

        context.CollectionProducts.Add(link);
        await context.SaveChangesAsync(cancellationToken);

        return Result<CollectionProductLinkResponse>.Success(MapResponse(link, product));
    }

    internal static CollectionProductLinkResponse MapResponse(CollectionProduct link, Product product) =>
        new(
            link.Id,
            link.CollectionId,
            link.ProductId,
            link.SortOrder,
            link.CreatedAt,
            new CollectionProductInfo(
                product.NameEn,
                product.NameAr,
                product.Slug,
                product.Price));
}
