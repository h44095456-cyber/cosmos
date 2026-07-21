using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Cosmos.App.ViewModels;
using System;
using Windows.Storage.Pickers;

namespace Cosmos.App;

public sealed partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        Title = "Cosmos PDF Editor";

        if (AppWindow is not null)
        {
            AppWindow.Resize(new Windows.Graphics.SizeInt32(1400, 900));
        }

        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(TitleBar);
    }

    private async void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.ViewMode = PickerViewMode.List;
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add(".pdf");

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            await _viewModel.LoadDocumentAsync(file.Path);
        }
    }

    private async void SaveFile_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_viewModel.DocumentPath))
            return;

        // TODO: Implement save functionality with iText7
        var dialog = new ContentDialog
        {
            Title = "Save",
            Content = "Save functionality will be implemented in Phase 2.",
            PrimaryButtonText = "OK",
            XamlRoot = this.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    private void PageThumbnails_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PageThumbnails.SelectedItem is PageThumbnail selected)
        {
            _viewModel.GoToPage(selected.PageNumber);
        }
    }

    private void DocumentCanvas_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case Windows.System.VirtualKey.Left:
            case Windows.System.VirtualKey.Up:
                _viewModel.PreviousPageCommand.Execute(null);
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Right:
            case Windows.System.VirtualKey.Down:
            case Windows.System.VirtualKey.Space:
                _viewModel.NextPageCommand.Execute(null);
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Home:
                _viewModel.GoToPage(1);
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.End:
                _viewModel.GoToPage(_viewModel.TotalPages);
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Add:
            case Windows.System.VirtualKey.OemPlus:
                if (e.KeyStatus.IsMenuKeyDown) // Ctrl key
                {
                    _viewModel.ZoomInCommand.Execute(null);
                    e.Handled = true;
                }
                break;
            case Windows.System.VirtualKey.Subtract:
            case Windows.System.VirtualKey.OemMinus:
                if (e.KeyStatus.IsMenuKeyDown) // Ctrl key
                {
                    _viewModel.ZoomOutCommand.Execute(null);
                    e.Handled = true;
                }
                break;
        }
    }
}
