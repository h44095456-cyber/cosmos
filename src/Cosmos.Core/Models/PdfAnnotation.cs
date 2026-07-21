namespace Cosmos.Core.Models;

public abstract class PdfAnnotation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int PageIndex { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
}
