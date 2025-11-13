using DataGraphControl.Core;
using DataGraphControl.WinUI.Acceleration;
using GhostCore.Data.Evaluation;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.UI;
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

public partial class ConnectionRenderData(Connection Data,
    float Thickness,
    Color Color,
    Vector2 Start,
    Vector2 End,
    Vector2 Control1,
    Vector2 Control2) : ElementRenderContext
{
    public Connection Data { get; } = Data;
    public float Thickness { get; set; } = Thickness;
    public Color Color { get; set; } = Color;
    public Vector2 Start { get; set; } = Start;
    public Vector2 End { get; set; } = End;
    public Vector2 Control1 { get; set; } = Control1;
    public Vector2 Control2 { get; set; } = Control2;
}

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

public sealed partial class CanvasBrushWithStates : IDisposable
{
    public ICanvasBrush NormalBrush { get; set; }
    public ICanvasBrush? MouseOverBrush { get; set; }
    public ICanvasBrush? PressedBrush { get; set; }
    public ICanvasBrush? SelectedBrush { get; set; }

    public CanvasBrushWithStates(ICanvasBrush normal, ICanvasBrush? selected = null)
    {
        NormalBrush = normal;
        SelectedBrush = selected;
    }

    public void Dispose()
    {
        NormalBrush?.Dispose();
        MouseOverBrush?.Dispose();
        PressedBrush?.Dispose();
        SelectedBrush?.Dispose();
    }
}