using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace CopyLess.Behaviors;

public class WindowDragger : Behavior<Window>
{
  protected override void OnAttached()
  {
    base.OnAttached();
    AssociatedObject?.PointerPressed += OnPointerPressed;
  }

  protected override void OnDetaching()
  {
    AssociatedObject?.PointerPressed -= OnPointerPressed;
    base.OnDetaching();
  }

  private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
  {
    if (e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed) AssociatedObject?.BeginMoveDrag(e);
  }
}