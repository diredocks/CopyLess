using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CopyLess.Services.ClipboardService;

namespace CopyLess.ViewModels;

public partial class ItemListViewModel : ViewModelBase
{
  // TODO: Refactor with CommunityToolKit.Mvvm 8.4.0
  private readonly ClipboardService _cs;

  private readonly ObservableCollection<ItemViewModel> _allItems = new();
  [ObservableProperty] private ObservableCollection<ItemViewModel> _items = new();

  [ObservableProperty] private string? _copiedText;

  [ObservableProperty] private ItemViewModel _selectedItem = new();

  [ObservableProperty] public partial string? Filter { get; set; }

  public ItemListViewModel(ClipboardService c)
  {
    _cs = c;
    _cs.ClipboardContentChanged += OnClipboardContentChanged;

    _allItems.CollectionChanged += (_, _) =>
    {
      UpdateItems();
      clearClipboardCommand?.NotifyCanExecuteChanged();
    };
  }

  private void OnItemPinnedChanged(object? sender, EventArgs e)
  {
    UpdateItems();
    clearClipboardCommand?.NotifyCanExecuteChanged();
  }

  partial void OnFilterChanged(string? value)
  {
    UpdateItems();
  }

  private void UpdateItems()
  {
    // Ordering items
    Items = _allItems
      .OrderByDescending(i => i.Pinned == true)
      .Where(i => i.Text.Contains(Filter ?? ""))
      // .ThenByDescending(i => i.CreatedAt) // TODO: Setting if order by create time
      .ToObservableCollection();
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
    newItem.PinnedChanged += OnItemPinnedChanged;
    _allItems.Insert(0, newItem);
    SelectedItem = newItem;
  }

  [RelayCommand]
  private void TogglePin(ItemViewModel item)
  {
    item.Pinned = !item.Pinned;
  }

  private bool CanClear => _allItems.Count - _allItems.Count(c => c.Pinned ?? false) > 0;
  
  [RelayCommand(CanExecute = nameof(CanClear))]
  private async Task ClearClipboardAsync()
  {
    await _cs.ClearAsync();
    // var itemsToRemove = _allItems.Where(c => c.Pinned != true).ToList();
    // foreach (var item in itemsToRemove)
    // {
    //   item.PinnedChanged -= OnItemPinnedChanged;
    //   _allItems.Remove(item);
    // }
    _allItems.Remove(i => i.Pinned == false);
    // WARN: This might cause mem leak since item hasn't unregistered event
    // causing event keeps reference to the item
    CopiedText = string.Empty;
  }
}