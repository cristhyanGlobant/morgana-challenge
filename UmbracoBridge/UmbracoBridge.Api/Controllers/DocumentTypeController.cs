using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UmbracoBridge.Application.DTOs;
using UmbracoBridge.Application.Interfaces;

namespace UmbracoBridge.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentTypeController : ControllerBase
{
    private readonly IUmbracoService _umbracoService;
    private readonly IValidator<DocumentTypeRequestDto> _validator;

    public DocumentTypeController(IUmbracoService umbracoService, IValidator<DocumentTypeRequestDto> validator)
    {
        _umbracoService = umbracoService;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocumentType([FromBody] DocumentTypeRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var response = await _umbracoService.CreateDocumentTypeAsync(request);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDocumentType(Guid id)
    {
        await _umbracoService.DeleteDocumentTypeAsync(id);
        return NoContent();
    }
}
