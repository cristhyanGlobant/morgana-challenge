using FluentValidation.TestHelper;
using UmbracoBridge.Application.DTOs;
using UmbracoBridge.Application.Validators;
using Xunit;

namespace UmbracoBridge.Tests.Validators;

public class DocumentTypeRequestValidatorTests
{
    private readonly DocumentTypeRequestValidator _validator;

    public DocumentTypeRequestValidatorTests()
    {
        _validator = new DocumentTypeRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var model = new DocumentTypeRequestDto { Name = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var model = new DocumentTypeRequestDto { Description = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Icon_Is_Empty()
    {
        var model = new DocumentTypeRequestDto { Icon = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Icon)
              .WithErrorMessage("Icon must start with 'icon-'.");
    }

    [Fact]
    public void Should_Have_Error_When_Icon_Does_Not_Start_With_IconDash()
    {
        var model = new DocumentTypeRequestDto { Icon = "custom-icon" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Icon)
              .WithErrorMessage("Icon must start with 'icon-'.");
    }

    [Fact]
    public void Should_Not_Have_Errors_When_All_Fields_Are_Valid()
    {
        var model = new DocumentTypeRequestDto
        {
            Name = "MyType",
            Description = "A valid description",
            Icon = "icon-document"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
