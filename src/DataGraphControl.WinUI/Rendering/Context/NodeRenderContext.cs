using DataGraphControl.Core;
using DataGraphControl.WinUI.Rendering.Context;
using DataGraphControl.WinUI.Rendering.Default;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Text;

namespace DataGraphControl.WinUI.Rendering;

internal sealed partial class NodeRenderContext : ElementRenderContext
{
    private CanvasTextFormat? _titleFormat;
    private CanvasTextLayout? _titleLayout;

    private ICanvasBrush? _backgroundBrush;
    private ICanvasBrush? _titleBrush;
    private CanvasBrushWithStates? _borderBrush;

    public ICanvasBrush? BackgroundBrush
    {
        get => _backgroundBrush; 
        set => SetDisposableProperty(value, ref _backgroundBrush);
    }
    public ICanvasBrush? TitleBrush
    {
        get => _titleBrush;
        set => SetDisposableProperty(value, ref _titleBrush);
    }
    public CanvasBrushWithStates? BorderStateBrush
    {
        get => _borderBrush;
        set => SetDisposableProperty(value, ref _borderBrush);
    }

    public CanvasTextFormat? TitleFormat
    {
        get => _titleFormat;
        set => SetDisposableProperty(value, ref _titleFormat);
    }
    public CanvasTextLayout? TitleLayout
    {
        get => _titleLayout;
        set => SetDisposableProperty(value, ref _titleLayout);
    }
    public NodeStyleRenderInfo Style { get; set; }
    public Vector2 TitlePosition { get; set; }

    public List<PropertyRenderContext> InputProperties { get; set; } = [];
    public List<PropertyRenderContext> OutputProperties { get; set; } = [];

    public Rect Rect => new(Position.ToPoint(), new Size(Size.X, Size.Y));

    public INode Data { get; set; }

    public NodeRenderContext(INode data, NodeStyleRenderInfo? nodeStyle = null)
    {
        Data = data;
        Style = nodeStyle ?? NodeStyleDefinition.Default.GetRenderInfo();

        TitleFormat = new CanvasTextFormat()
        {
            WordWrapping = CanvasWordWrapping.NoWrap,
            FontFamily = Style.TitleTextProperties?.FontFamily ?? "Segoe UI",
            FontSize = Style.TitleTextProperties?.FontSize ?? 12,
            FontStyle = (FontStyle)(Style.TitleTextProperties?.Italic ?? 0),
            FontWeight = new FontWeight((ushort)(Style.TitleTextProperties?.FontWeight ?? 400))
        };
    }

    public override void Dispose()
    {
        foreach (var item in InputProperties)
            item.Dispose();

        foreach (var item in OutputProperties)
            item.Dispose();

        base.Dispose();
    }
}
