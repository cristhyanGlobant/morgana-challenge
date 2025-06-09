namespace UmbracoBridge.Application.DTOs;

public class DocumentTypeRequestDto
{
    public string Alias { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool AllowAsRoot { get; set; }
    public bool VariesByCulture { get; set; }
    public bool VariesBySegment { get; set; }
    public bool IsElement { get; set; }

}
