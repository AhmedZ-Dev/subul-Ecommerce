using backend.Common.Results;
using MediatR;
using backend.Features.CategoryFeature.GetByIdCategory;

namespace backend.Features.CategoryFeature.GetBySlugCategory;

public record GetBySlugCategoryQuery(string Slug) : IRequest<Result<GetByIdCategoryResponse>>;
