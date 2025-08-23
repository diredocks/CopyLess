using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CopyLess.ViewModels;

public partial class ItemViewModel : ViewModelBase
{
  public DateTime? CreatedAt { get; } = DateTime.Now;
  [ObservableProperty] private bool? _pinned;
  [ObservableProperty] private string? _text;
}