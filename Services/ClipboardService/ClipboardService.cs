using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using IDataObject = Avalonia.Input.IDataObject;

namespace CopyLess.Services.ClipboardService;

public abstract class ClipboardService : IClipboardService, IDisposable
{
  private IClipboard? Clipboard => field ??= Application.Current.GetTopLevel()?.Clipboard;

  protected TopLevel? _topLevel => Application.Current.GetTopLevel();
  public event EventHandler<EventArgs>? ClipboardContentChanged;

  public abstract void Initialize();
  public abstract void Dispose();

  public Task<string?> GetTextAsync()
  {
    return Clipboard?.GetTextAsync() ?? Task.FromResult<string?>(null);
  }

  public Task SetTextAsync(string? text)
  {
    return Clipboard?.SetTextAsync(text) ?? Task.CompletedTask;
  }

  public Task ClearAsync()
  {
    return Clipboard?.ClearAsync() ?? Task.CompletedTask;
  }

  public Task SetDataObjectAsync(IDataObject data)
  {
    return Clipboard?.SetDataObjectAsync(data) ?? Task.CompletedTask;
  }

  public Task<string[]> GetFormatsAsync()
  {
    return Clipboard?.GetFormatsAsync() ?? Task.FromResult(Array.Empty<string>());
  }

  public Task<object?> GetDataAsync(string format)
  {
    return Clipboard?.GetDataAsync(format) ?? Task.FromResult<object?>(null);
  }

  protected void OnClipboardContentChanged()
  {
    ClipboardContentChanged?.Invoke(this, EventArgs.Empty);
  }
}

public static class ClipboardServiceFactory
{
  public static ClipboardService CreateClipboardService()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return new ClipboardServiceWindows();

    throw new PlatformNotSupportedException("Clipboard monitoring not supported on this platform");
  }
}