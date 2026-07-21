using System.Threading.Tasks;

namespace Cosmos.Rendering;

public interface IPdfRenderer
{
    int PageCount { get; }
    Task LoadDocumentAsync(string path);
    Task<byte[]?> RenderPageAsync(int pageIndex, float scale);
    Task<(int Width, int Height)> GetPageSizeAsync(int pageIndex);
}
