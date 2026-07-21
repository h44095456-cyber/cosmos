namespace Cosmos.Core.Models;

public class PdfDocument
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public DateTime LoadedAt { get; set; }
    public List<PdfPage> Pages { get; set; } = new();
    public List<PdfBookmark> Bookmarks { get; set; } = new();
    public PdfMetadata? Metadata { get; set; }
}
