using backend.Common.Results;
using MediatR;

namespace backend.Features.CategoryFeature.DeleteCategory;

public record DeleteCategoryCommand(long Id) : IRequest<Result<bool>>;
