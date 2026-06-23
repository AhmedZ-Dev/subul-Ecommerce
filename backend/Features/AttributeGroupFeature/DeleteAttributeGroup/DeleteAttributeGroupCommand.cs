using backend.Common.Results;
using MediatR;

namespace backend.Features.AttributeGroupFeature.DeleteAttributeGroup;

public record DeleteAttributeGroupCommand(long Id) : IRequest<Result<bool>>;
