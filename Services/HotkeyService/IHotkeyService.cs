using System;

namespace CopyLess.Services.HotkeyService;

public interface IHotkeyService
{
  event EventHandler<EventArgs> HotkeyActivated;
  void Initialize();
  void Dispose(); 
}