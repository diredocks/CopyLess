using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using CopyLess.Services.ClipboardService;

namespace CopyLess.Services.HotkeyService;

public abstract class HotkeyService: IHotkeyService, IDisposable
{
  protected TopLevel? _topLevel => Application.Current.GetTopLevel();
  public event EventHandler<EventArgs>? HotkeyActivated;
  public abstract void Initialize();
  public abstract void Dispose();

  protected void OnHotkeyActivated()
  {
    HotkeyActivated?.Invoke(this, EventArgs.Empty);
  }
}

public static class HotkeyServiceFactory
{
  public static HotkeyService CreateClipboardService()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return new HotkeyServiceWindows();

    throw new PlatformNotSupportedException("Clipboard monitoring not supported on this platform");
  }
}