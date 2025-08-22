using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using CopyLess.Services.HotkeyService;
using CopyLess.Services.ClipboardService;
using CopyLess.ViewModels;
using CopyLess.Views;

namespace CopyLess;

public class App : Application
{
  public static ClipboardService? ClipboardService { get; private set; }
  public static HotkeyService? HotkeyService { get; private set; }

  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    ClipboardService = ClipboardServiceFactory.CreateClipboardService();
    HotkeyService = HotkeyServiceFactory.CreateClipboardService();

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
      // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
      DisableAvaloniaDataAnnotationValidation();
      desktop.MainWindow = new MainView(HotkeyService)
      {
        DataContext = new MainViewModel(ClipboardService)
      };

      ClipboardService.Initialize();
      HotkeyService.Initialize();
      desktop.MainWindow.Closing += OnExit;
    }
    
#if DEBUG
    this.AttachDevTools();
#endif

    base.OnFrameworkInitializationCompleted();
  }

  private void OnExit(object? sender, WindowClosingEventArgs e)
  {
    ClipboardService?.Dispose();
    HotkeyService?.Dispose();
  }

  private void DisableAvaloniaDataAnnotationValidation()
  {
    // Get an array of plugins to remove
    var dataValidationPluginsToRemove =
      BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

    // remove each entry found
    foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
  }
}

// workaround from https://github.com/AvaloniaUI/Avalonia/discussions/11170
public static class ApplicationExtensions
{
  /// <summary>
  ///   Returns the TopLevel from the main window or view.
  /// </summary>
  /// <param name="app">The application to get the TopLevel for.</param>
  /// <returns>A TopLevel object.</returns>
  public static TopLevel? GetTopLevel(this Application? app)
  {
    if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) return desktop.MainWindow;
    if (app?.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
    {
      var visualRoot = viewApp.MainView?.GetVisualRoot();
      return visualRoot as TopLevel;
    }

    return null;
  }
}