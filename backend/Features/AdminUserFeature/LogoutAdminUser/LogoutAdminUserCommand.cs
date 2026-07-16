using backend.Common.Results;
using MediatR;

namespace backend.Features.AdminUserFeature.LogoutAdminUser;

public record LogoutAdminUserCommand : IRequest<Result<LogoutAdminUserResponse>>;

public record LogoutAdminUserResponse(bool Success);
