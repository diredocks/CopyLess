using CopyLess.Services.ClipboardService;

namespace CopyLess.ViewModels;

public class MainViewModel : ViewModelBase
{
  public ItemListViewModel ItemListVM { get; set; }
  
  public MainViewModel(ClipboardService c)
  {
    ItemListVM = new ItemListViewModel(c);
  }
}