using DataGraphControl.WinUI.Rendering.Context;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;

namespace DataGraphControl.WinUI.Rendering;

public sealed partial class PropertyRenderContext : ElementRenderContext
{
    private CanvasTextFormat? _textFormat;
    private CanvasTextLayout? _textLayout;

    private CanvasBrushWithStates? _portBrush;
    private ICanvasBrush? _textBrush;

    public uint Id { get; set; }

    public CanvasBrushWithStates? PortBrush
    {
        get => _portBrush;
        set => SetDisposableProperty(value, ref _portBrush);
    }
    public ICanvasBrush? TextBrush
    {
        get => _textBrush;
        set => SetDisposableProperty(value, ref _textBrush);
    }

    public CanvasTextFormat? TextFormat
    {
        get => _textFormat;
        set => SetDisposableProperty(value, ref _textFormat);
    }
    public CanvasTextLayout? TextLayout
    {
        get => _textLayout;
        set => SetDisposableProperty(value, ref _textLayout);
    }

    public Vector2 PortPosition { get; set; }
    public float Radius { get; set; }
}
