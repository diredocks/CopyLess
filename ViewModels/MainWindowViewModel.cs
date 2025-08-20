using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CopyLess.Services.ClipboardService;

namespace CopyLess.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
  private readonly ClipboardService cs;

  [ObservableProperty] private ObservableCollection<CardViewModel> _cards = new();

  [ObservableProperty] private string? _copiedText;

  public MainWindowViewModel(ClipboardService c)
  {
    cs = c;
    cs.ClipboardContentChanged += OnClipboardContentChanged;
    Cards.CollectionChanged += (_, _) => { clearClipboardCommand?.NotifyCanExecuteChanged(); };
  }

  private bool CanClear => _cards.Count > 0;

  private async void OnClipboardContentChanged(object? sender, EventArgs e)
  {
    var text = await cs.GetTextAsync();

    if (string.IsNullOrWhiteSpace(text))
      return;
    if (text == CopiedText)
      return;

    CopiedText = text;
    Cards.Insert(0, new CardViewModel { Text = text });
  }

  [RelayCommand(CanExecute = nameof(CanClear))]
  private async Task ClearClipboardAsync()
  {
    await cs.ClearAsync();
    Cards.Clear();
  }
}