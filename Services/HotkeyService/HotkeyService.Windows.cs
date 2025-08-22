using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace CopyLess.Services.HotkeyService;

public class HotkeyServiceWindows : HotkeyService
{
  private const uint WmHotkey = 0x0312; // Message ID for the hotkey message
  private int _keyId = 9000; // ID for the hotkey to be registered.

  [DllImport("user32.dll")]
  private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

  [DllImport("user32.dll")]
  private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

  public override void Initialize()
  {
    if (_topLevel is null) return;

    var platformHandle = _topLevel.TryGetPlatformHandle();
    if (platformHandle is null) return;

    var res = RegisterHotKey(platformHandle.Handle, _keyId, (uint)(WinModifierKeys.Alt | WinModifierKeys.Shift),
      (uint)WinKey.W);
    if (!res) throw new InvalidOperationException("Failed to register hotkey");

    Win32Properties.AddWndProcHookCallback(_topLevel, WndProc);
  }

  private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    if (msg == WmHotkey && wParam == _keyId) OnHotkeyActivated();

    return IntPtr.Zero;
  }

  public override void Dispose()
  {
    if (_topLevel is null) return;

    var platformHandle = _topLevel.TryGetPlatformHandle();
    if (platformHandle is not null) UnregisterHotKey(platformHandle.Handle, _keyId);
  }
}