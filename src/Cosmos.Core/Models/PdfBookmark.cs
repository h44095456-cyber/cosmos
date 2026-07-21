namespace Cosmos.Core.Models;

public class PdfBookmark
{
    public string Title { get; set; } = string.Empty;
    public int PageIndex { get; set; }
    public List<PdfBookmark> Children { get; set; } = new();
}
