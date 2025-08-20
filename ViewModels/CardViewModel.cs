using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CopyLess.ViewModels;

public partial class CardViewModel : ViewModelBase
{
  [ObservableProperty] private bool? _pinned;
  [ObservableProperty] private string? _text;

  [RelayCommand]
  private void TogglePinned()
  {
    Pinned = !Pinned;
  }
}