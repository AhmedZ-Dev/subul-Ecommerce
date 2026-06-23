using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.AttributeGroupFeature;

[Collection("Database")]
public class CreateAttributeGroupHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeGroupHandler(context);
        var command = new CreateAttributeGroupCommand(
            NameEn: "Processor Specs",
            NameAr: "مواصفات المعالج",
            SortOrder: 1,
            IsFilterable: true
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Processor Specs", result.Value.NameEn);
        Assert.Equal("processor-specs", result.Value.Slug);
    }

    [Fact]
    public async Task Handle_WithAttributes_CreatesNestedAttributes()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeGroupHandler(context);
        var command = new CreateAttributeGroupCommand(
            NameEn: "Display Details " + Guid.NewGuid(),
            Attributes: new List<CreateAttributeGroupAttributeInput>
            {
                new("Screen Size", null, null, "inch", "number", true, 1),
                new("Refresh Rate", null, null, "Hz", "select", true, 2)
            }
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Attributes.Count);
        Assert.Equal("Screen Size", result.Value.Attributes[0].NameEn);
        Assert.Equal("screen-size", result.Value.Attributes[0].Slug);
        Assert.Equal("number", result.Value.Attributes[0].InputType);
        Assert.Equal("refresh-rate", result.Value.Attributes[1].Slug);
        Assert.Equal("select", result.Value.Attributes[1].InputType);
    }

    [Fact]
    public async Task Handle_DuplicateGroupName_ReturnsConflict()
    {
        await using var context = fixture.CreateContext();
        var name = "Unique Name " + Guid.NewGuid();
        var handler = new CreateAttributeGroupHandler(context);
        var command = new CreateAttributeGroupCommand(NameEn: name);

        await handler.Handle(command, CancellationToken.None);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_InvalidInputType_ReturnsBadRequest()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeGroupHandler(context);
        var command = new CreateAttributeGroupCommand(
            NameEn: "Invalid Input Type Group " + Guid.NewGuid(),
            Attributes: new List<CreateAttributeGroupAttributeInput>
            {
                new("Bad Type Attribute", InputType: "unsupported-type")
            }
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid input type", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
