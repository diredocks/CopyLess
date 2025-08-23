using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CopyLess.Services.ClipboardService;
using ObservableCollections;

namespace CopyLess.ViewModels;

public partial class ItemListViewModel : ViewModelBase
{
  private readonly ClipboardService _cs;

  private readonly ObservableList<ItemViewModel> _items = new();
  private readonly ISynchronizedView<ItemViewModel, ItemViewModel> _syncView;
  public INotifyCollectionChangedSynchronizedViewList<ItemViewModel> Items { get; }

  [ObservableProperty] private string? _copiedText;

  [ObservableProperty] private ItemViewModel _selectedItem = new();

  [ObservableProperty] public partial string? Filter { get; set; }

  public ItemListViewModel(ClipboardService c)
  {
    _cs = c;
    _cs.ClipboardContentChanged += OnClipboardContentChanged;

    _syncView = _items.CreateView(i => i);
    Items = _syncView.ToNotifyCollectionChanged();
    Items.CollectionChanged += (_, _) => { clearClipboardCommand?.NotifyCanExecuteChanged(); };
  }

  private void OnItemPinnedChanged(object? sender, EventArgs e)
  {
    // _items.Sort(Comparer<ItemViewModel>.Create((a, b) => 
    //   (b.Pinned ?? false).CompareTo(a.Pinned ?? false)));
    clearClipboardCommand?.NotifyCanExecuteChanged();
  }

  partial void OnFilterChanged(string? value)
  {
    _syncView.AttachFilter(i => i.Text.Contains(Filter ?? ""));
  }

  private async void OnClipboardContentChanged(object? sender, EventArgs e)
  {
    var text = await _cs.GetTextAsync();

    if (string.IsNullOrWhiteSpace(text))
      return;
    if (text == CopiedText)
      return;

    CopiedText = text;
    var newItem = new ItemViewModel { Text = CopiedText, Pinned = false };
    _items.Insert(0, newItem); // New on top
    SelectedItem = newItem;
  }

  private bool CanClear => Items.Count() - Items.Count(c => c.Pinned ?? false) > 0;

  [RelayCommand(CanExecute = nameof(CanClear))]
  private async Task ClearClipboardAsync()
  {
    await _cs.ClearAsync();
    foreach (var i in Items.Where(i => i.Pinned == false).ToList())
    {
      _items.Remove(i);
    }

    CopiedText = string.Empty;
  }

  [RelayCommand]
  private void TogglePinItem(ItemViewModel i)
  {
    i.Pinned = !i.Pinned;
    clearClipboardCommand?.NotifyCanExecuteChanged();
  }

  [RelayCommand]
  private void CopyItem(ItemViewModel i)
  {
    Debug.WriteLine($"{i.Text ?? ""}");
    SelectedItem = i;
  }
}