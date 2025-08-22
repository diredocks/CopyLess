using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CopyLess.Views;

public partial class ItemListView : UserControl
{
  public ItemListView()
  {
    InitializeComponent();
  }

  private void Button_OnClick(object? sender, RoutedEventArgs e)
  {
    (TopLevel.GetTopLevel(this) as Window)?.Close(); // Close Window
  }
}