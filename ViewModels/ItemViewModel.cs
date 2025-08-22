using System;
using Avalonia.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CopyLess.ViewModels;

public partial class ItemViewModel : ViewModelBase
{
  public DateTime? CreatedAt { get; } = DateTime.Now;
  [ObservableProperty] private bool? _pinned;
  [ObservableProperty] private string? _text;

  public event EventHandler? PinnedChanged;

  [RelayCommand]
  private void TogglePinned()
  {
    Pinned = !Pinned;
    PinnedChanged?.Invoke(this, EventArgs.Empty);
  }
}