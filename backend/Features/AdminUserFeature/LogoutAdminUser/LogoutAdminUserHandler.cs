using backend.Common.Results;
using MediatR;

namespace backend.Features.AdminUserFeature.LogoutAdminUser;

public class LogoutAdminUserHandler
    : IRequestHandler<LogoutAdminUserCommand, Result<LogoutAdminUserResponse>>
{
    public Task<Result<LogoutAdminUserResponse>> Handle(
        LogoutAdminUserCommand request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Result<LogoutAdminUserResponse>.Success(new LogoutAdminUserResponse(true)));
    }
}
