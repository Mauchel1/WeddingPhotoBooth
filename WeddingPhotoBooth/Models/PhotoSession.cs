namespace WeddingPhotoBooth.Models;

public class PhotoSession
{
    public Guid Id { get; } = Guid.NewGuid();

    public string SessionFolder { get; set; } = "";

    public List<string> Photos { get; } = new();

    public string? StripFile { get; set; }
}