namespace UmbracoBridge.Application.DTOs;

public class HealthCheckResponse
{
    public int Total { get; set; }
    public List<HealthCheckItem> Items { get; set; } = new();
}

public class HealthCheckItem
{
    public string Name { get; set; } = string.Empty;
}
