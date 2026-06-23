using backend.Common.Results;
using MediatR;

namespace backend.Features.AddressFeature.DeleteAddress;

public record DeleteAddressCommand(long Id) : IRequest<Result<bool>>;
