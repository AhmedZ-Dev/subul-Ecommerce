using backend.Common.Results;
using backend.Features.AttributeFeature.CreateAttribute;
using MediatR;

namespace backend.Features.AttributeFeature.GetByIdAttribute;

public record GetByIdAttributeQuery(long GroupId, long Id)
    : IRequest<Result<AttributeResponse>>;
