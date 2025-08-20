using System;

namespace CopyLess.Services.ClipboardService;

public interface IClipboardService
{
  event EventHandler<EventArgs> ClipboardContentChanged;
  void Initialize();
  void Dispose();
}