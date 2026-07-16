using backend.Common.Results;
using backend.Features.AdminUserFeature.LoginAdminUser;
using MediatR;

namespace backend.Features.AdminUserFeature.GetCurrentAdminUser;

public record GetCurrentAdminUserQuery(long AdminUserId)
    : IRequest<Result<GetCurrentAdminUserResponse>>;

public record GetCurrentAdminUserResponse(AdminUserDto User);
