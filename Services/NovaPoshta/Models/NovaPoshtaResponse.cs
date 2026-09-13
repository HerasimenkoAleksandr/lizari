namespace lizari.Services.NovaPoshta.Models;

public class NovaPoshtaResponse<T>
{
    public bool Success { get; set; }

    public List<T> Data { get; set; } = new();

    public List<string> Errors { get; set; } = new();

    public List<string> Warnings { get; set; } = new();
}