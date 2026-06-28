using backend.Common.Results;
using MediatR;

namespace backend.Features.CategoryFeature.ChangeCategoryStatus;

public record ChangeCategoryStatusCommand(long Id, bool IsActive)
    : IRequest<Result<ChangeCategoryStatusResponse>>;

public record ChangeCategoryStatusRequest(bool IsActive);

public record ChangeCategoryStatusResponse(
    long Id,
    bool IsActive,
    DateTime? UpdatedAt);
