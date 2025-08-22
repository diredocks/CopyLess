using Avalonia.Controls;
using Avalonia.Input;
using CopyLess.Services.HotkeyService;

namespace CopyLess.Views;

public partial class MainView : Window
{
  private readonly HotkeyService? _hotkeyService;

  public MainView(HotkeyService h)
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
    // TODO: Implement lose focus hide
    // https://github.com/DearVa/Everywhere/compare/v0.1.2...v0.1.3
    KeyDown += (_, e) =>
    {
      if (e.Key == Key.Escape) Hide();
    };
  }
}