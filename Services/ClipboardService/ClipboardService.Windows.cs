using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace CopyLess.Services.ClipboardService;

public class ClipboardServiceWindows : ClipboardService
{
  private const uint WM_DRAWCLIPBOARD = 0x0308;
  private IntPtr _clipboardViewerHandle;

  [DllImport("user32.dll", SetLastError = true)]
  private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

  [DllImport("user32.dll", SetLastError = true)]
  private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

  public override void Initialize()
  {
    if (_topLevel is null) return;

    Win32Properties.AddWndProcHookCallback(_topLevel, WndProc);

    var platformHandle = _topLevel.TryGetPlatformHandle();
    if (platformHandle is null) return;

    _clipboardViewerHandle = SetClipboardViewer(platformHandle.Handle);
  }

  private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    if (msg == WM_DRAWCLIPBOARD) OnClipboardContentChanged();

    return IntPtr.Zero;
  }

  public override void Dispose()
  {
    if (_topLevel is null) return;

    var platformHandle = _topLevel.TryGetPlatformHandle();
    if (platformHandle is not null) ChangeClipboardChain(platformHandle.Handle, _clipboardViewerHandle);
  }
}