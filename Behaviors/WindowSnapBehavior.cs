using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace CopyLess.Behaviors;

public enum SnapEdge
{
  None,
  Left,
  Top,
  Right,
  Bottom
}

public class WindowSnapBehavior : Behavior<Window>
{
  public static readonly StyledProperty<int> HoverSizeProperty =
    AvaloniaProperty.Register<WindowSnapBehavior, int>(nameof(HoverSize), 3);

  private static readonly StyledProperty<SnapEdge> SnapEdgeProperty =
    AvaloniaProperty.Register<WindowSnapBehavior, SnapEdge>(nameof(SnapEdge));

  public int HoverSize
  {
    get => GetValue(HoverSizeProperty);
    set => SetValue(HoverSizeProperty, value);
  }

  public SnapEdge SnapEdge
  {
    get => GetValue(SnapEdgeProperty);
    private set => SetValue(SnapEdgeProperty, value);
  }

  private int TotalScreenWidth =>
    AssociatedObject?.Screens.All.Sum(screen => screen.WorkingArea.Width) ?? 0;

  protected override void OnAttached()
  {
    base.OnAttached();

    AssociatedObject?.PointerPressed += OnPointerPressed;
    AssociatedObject?.PointerReleased += OnPointerReleased;
    AssociatedObject?.PointerEntered += OnPointerEntered;
    AssociatedObject?.PointerExited += OnPointerExited;
  }

  protected override void OnDetaching()
  {
    AssociatedObject?.PointerPressed -= OnPointerPressed;
    AssociatedObject?.PointerReleased -= OnPointerReleased;
    AssociatedObject?.PointerEntered -= OnPointerEntered;
    AssociatedObject?.PointerExited -= OnPointerExited;

    base.OnDetaching();
  }

  private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
  {
    if (e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed) AssociatedObject?.BeginMoveDrag(e);
  }

  private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
  {
    var (newPosition, edge) = GetClampedPositionAndEdge();
    if (AssociatedObject != null)
    {
      AssociatedObject.Position = newPosition;
      SnapEdge = edge;
    }
  }

  private void OnPointerEntered(object? sender, PointerEventArgs e)
  {
    if (SnapEdge != SnapEdge.None && AssociatedObject != null)
      AssociatedObject.Position = ClampToScreenBounds(AssociatedObject.Position);
  }

  private void OnPointerExited(object? sender, PointerEventArgs e)
  {
    if (SnapEdge != SnapEdge.None && AssociatedObject != null) AssociatedObject.Position = GetHiddenPosition(SnapEdge);
  }

  private (PixelPoint position, SnapEdge edge) GetClampedPositionAndEdge()
  {
    if (AssociatedObject == null)
      return (new PixelPoint(0, 0), SnapEdge.None);

    var (clampedX, clampedY, edge) = ClampToClosestEdge(
      AssociatedObject.Position.X,
      AssociatedObject.Position.Y);

    var newPosition = new PixelPoint(clampedX, clampedY);

    return AssociatedObject.Position == newPosition
      ? (AssociatedObject.Position, SnapEdge.None)
      : (newPosition, edge);
  }

  private PixelPoint ClampToScreenBounds(PixelPoint position)
  {
    if (AssociatedObject == null)
      return position;

    var clampedX = Math.Clamp(position.X, 0, TotalScreenWidth - (int)AssociatedObject.Width);
    var clampedY = Math.Clamp(position.Y, 0,
      AssociatedObject.Screens.Primary?.Bounds.Height - (int)AssociatedObject.Height ?? 0);

    return new PixelPoint(clampedX, clampedY);
  }

  private PixelPoint GetHiddenPosition(SnapEdge edge)
  {
    if (AssociatedObject == null)
      return new PixelPoint(0, 0);

    return edge switch
    {
      SnapEdge.Left => new PixelPoint(-(int)AssociatedObject.Width + HoverSize, AssociatedObject.Position.Y),
      SnapEdge.Top => new PixelPoint(AssociatedObject.Position.X, -(int)AssociatedObject.Height + HoverSize),
      SnapEdge.Right => new PixelPoint(TotalScreenWidth - HoverSize, AssociatedObject.Position.Y),
      SnapEdge.Bottom => new PixelPoint(AssociatedObject.Position.X,
        (AssociatedObject.Screens.Primary?.Bounds.Height ?? 0) - HoverSize),
      _ => AssociatedObject.Position
    };
  }

  private (int x, int y, SnapEdge edge) ClampToClosestEdge(int x, int y)
  {
    if (AssociatedObject == null)
      return (x, y, SnapEdge.None);

    var (w, h) = ((int)AssociatedObject.Width, (int)AssociatedObject.Height);
    var screenHeight = AssociatedObject.Screens.Primary?.Bounds.Height ?? 0;

    var distances = new[]
    {
      (distance: x, edge: SnapEdge.Left, position: (0, y)),
      (distance: TotalScreenWidth - x - w, edge: SnapEdge.Right, position: (TotalScreenWidth - w, y)),
      (distance: y, edge: SnapEdge.Top,
        position: (x, -1)), // workaround since Windows would prevent it getting too high
      (distance: screenHeight - y - h, edge: SnapEdge.Bottom, position: (x, screenHeight - h))
    };

    var closest = distances.MinBy(d => d.distance);

    return closest.distance > HoverSize
      ? (x, y, SnapEdge.None)
      : (closest.position.Item1, closest.position.Item2, closest.edge);
  }
}