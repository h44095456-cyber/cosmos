using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cosmos.Rendering;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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

    [ObservableProperty]
    private BitmapImage? _currentPageImage;

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
        await RenderCurrentPageAsync();
    }

    private async Task GenerateThumbnailsAsync()
    {
        Pages.Clear();

        for (int i = 0; i < TotalPages; i++)
        {
            var thumbnailData = await _renderer.RenderPageAsync(i, 0.2f);
            BitmapImage? bitmap = null;

            if (thumbnailData != null)
            {
                bitmap = new BitmapImage();
                using var stream = new MemoryStream(thumbnailData);
                await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
            }

            Pages.Add(new PageThumbnail
            {
                PageNumber = i + 1,
                ThumbnailImage = bitmap
            });
        }
    }

    private async Task RenderCurrentPageAsync()
    {
        if (CurrentPage < 1 || CurrentPage > TotalPages)
            return;

        float scale = ZoomLevel / 100.0f;
        var pageData = await _renderer.RenderPageAsync(CurrentPage - 1, scale);

        if (pageData != null)
        {
            var bitmap = new BitmapImage();
            using var stream = new MemoryStream(pageData);
            await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
            CurrentPageImage = bitmap;
        }
    }

    partial void OnCurrentPageChanged(int value)
    {
        _ = RenderCurrentPageAsync();
    }

    partial void OnZoomLevelChanged(int value)
    {
        _ = RenderCurrentPageAsync();
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

    [RelayCommand]
    public void GoToPage(int pageNumber)
    {
        if (pageNumber >= 1 && pageNumber <= TotalPages)
        {
            CurrentPage = pageNumber;
        }
    }
}

public class PageThumbnail
{
    public int PageNumber { get; set; }
    public BitmapImage? ThumbnailImage { get; set; }
}
