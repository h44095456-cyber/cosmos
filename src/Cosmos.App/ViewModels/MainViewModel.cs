using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cosmos.Rendering;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cosmos.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IPdfRenderer _renderer;

    [ObservableProperty]
    private int _currentPage = 0;

    [ObservableProperty]
    private int _totalPages = 0;

    [ObservableProperty]
    private int _zoomLevel = 100;

    [ObservableProperty]
    private string? _documentPath;

    public ObservableCollection<PageThumbnail> Pages { get; } = new();

    public MainViewModel(IPdfRenderer renderer)
    {
        _renderer = renderer;
    }

    [RelayCommand]
    public async Task LoadDocumentAsync(string path)
    {
        DocumentPath = path;

        await _renderer.LoadDocumentAsync(path);
        TotalPages = _renderer.PageCount;
        CurrentPage = 1;

        await GenerateThumbnailsAsync();
    }

    private async Task GenerateThumbnailsAsync()
    {
        Pages.Clear();

        for (int i = 0; i < TotalPages; i++)
        {
            var thumbnail = await _renderer.RenderPageAsync(i, 0.2f);
            Pages.Add(new PageThumbnail
            {
                PageNumber = i + 1,
                Thumbnail = thumbnail
            });
        }
    }

    [RelayCommand]
    public void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
        }
    }

    [RelayCommand]
    public void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
        }
    }

    [RelayCommand]
    public void ZoomIn()
    {
        if (ZoomLevel < 400)
        {
            ZoomLevel += 25;
        }
    }

    [RelayCommand]
    public void ZoomOut()
    {
        if (ZoomLevel > 25)
        {
            ZoomLevel -= 25;
        }
    }
}

public class PageThumbnail
{
    public int PageNumber { get; set; }
    public byte[]? Thumbnail { get; set; }
}
