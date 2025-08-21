using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CopyLess.Services.HotkeyService;

namespace CopyLess.Views;

public partial class MainWindow : Window
{
  private readonly HotkeyService? _hotkeyService;
  public MainWindow(HotkeyService h)
  {
    InitializeComponent();
    _hotkeyService = h;
    _hotkeyService.HotkeyActivated += (_, _) =>
    {
      if (!IsVisible)
      {
        Show();
        Activate();
      }
      else
      {
        Hide();
      }
    };
    // Deactivated += (_, _) =>
    // {
    //   Hide();
    // };
    KeyDown += (_, e) =>
    {
      if (e.Key == Key.Escape)
      {
        Hide();
      }
    };
  }

  private void Button_OnClick(object? sender, RoutedEventArgs e)
  {
    Close();
  }
}