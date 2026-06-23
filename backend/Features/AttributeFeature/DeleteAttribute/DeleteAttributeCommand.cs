using backend.Common.Results;
using MediatR;

namespace backend.Features.AttributeFeature.DeleteAttribute;

public record DeleteAttributeCommand(long GroupId, long Id) : IRequest<Result<bool>>;
