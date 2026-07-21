namespace Cosmos.Core.Models;

public class PdfPage
{
    public int Index { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Rotation { get; set; }
    public List<PdfAnnotation> Annotations { get; set; } = new();
}
