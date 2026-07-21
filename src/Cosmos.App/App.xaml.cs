using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Cosmos.App.ViewModels;
using Cosmos.Rendering;

namespace Cosmos.App;

public partial class App : Application
{
    private Window? _window;
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IPdfRenderer, PdfiumRenderer>();
        services.AddTransient<MainViewModel>();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow(_serviceProvider.GetRequiredService<MainViewModel>());
        _window.Activate();
    }
}
