using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CopyLess.Services.ClipboardService;

namespace CopyLess.ViewModels;

public partial class MainViewModel : ViewModelBase
{
  private readonly ClipboardService _cs;

  [ObservableProperty] private CardViewModel _selectedCard = new();
  [ObservableProperty] private ObservableCollection<CardViewModel> _cards = new();

  [ObservableProperty] private string? _copiedText;

  public MainViewModel(ClipboardService c)
  {
    _cs = c;
    _cs.ClipboardContentChanged += OnClipboardContentChanged;
    Cards.CollectionChanged += (_, _) => { clearClipboardCommand?.NotifyCanExecuteChanged(); };
  }

  private bool CanClear => Cards.Count - Cards.Count(c => c.Pinned ?? false) > 0;

  private async void OnClipboardContentChanged(object? sender, EventArgs e)
  {
    var text = await _cs.GetTextAsync();

    if (string.IsNullOrWhiteSpace(text))
      return;
    if (text == CopiedText)
      return;

    CopiedText = text;
    var newCard = new CardViewModel { Text = CopiedText, Pinned = false };
    newCard.PinnedChanged += (_, _) => { clearClipboardCommand?.NotifyCanExecuteChanged(); };
    Cards.Insert(0, newCard);
    SelectedCard = newCard;
  }

  [RelayCommand(CanExecute = nameof(CanClear))]
  private async Task ClearClipboardAsync()
  {
    await _cs.ClearAsync();
    Cards.Remove(c => !c.Pinned ?? false);
    CopiedText = string.Empty;
  }
}