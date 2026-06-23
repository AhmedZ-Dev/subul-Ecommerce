using backend.Common.Results;
using MediatR;

namespace backend.Features.ShippingZoneFeature.DeleteShippingZone;

public record DeleteShippingZoneCommand(long Id) : IRequest<Result<bool>>;
