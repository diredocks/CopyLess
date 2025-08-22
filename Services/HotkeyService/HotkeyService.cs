using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;

namespace CopyLess.Services.HotkeyService;

public abstract class HotkeyService : IHotkeyService, IDisposable
{
  // TODO: Multiple hotkeys registration
  // TODO: Initialization should allow custom key
  // TODO: Event should deliver activated hotkey via args
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

    throw new PlatformNotSupportedException("Global Hotkey not supported on this platform");
  }
}