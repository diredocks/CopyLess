using CommunityToolkit.Mvvm.ComponentModel;

namespace CopyLess.ViewModels;

public partial class CardViewModel : ViewModelBase
{
  [ObservableProperty] private bool? _pinned;
  [ObservableProperty] private string? _text;
}