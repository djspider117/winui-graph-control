using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataGraphControl.WinUI;

internal interface IRenderInfo { }

internal class NodeStyleRenderInfo : IRenderInfo
{
    internal Vector2 CornerRadius { get; set; }
    internal Vector2 MinMaxWidth { get; set; }
    internal Vector2 MinMaxHeight { get; set; }

    internal int RowHeight { get; set; }

    public float BorderThickness { get; set; }
    internal IBrushProxy? BorderBrush { get; set; }
    internal IBrushProxy? BackgroundBrush { get; set; }
    internal PortStyleRenderInfo? InputPortStyle { get; set; }
    internal PortStyleRenderInfo? OutputPortStyle { get; set; }

    internal IBrushProxy? TitleBrush { get; set; }
    internal TextPropertiesRenderInfo? TitleTextProperties { get; set; }

    internal bool ShowPorts { get; set; }
    internal Orientation Orientation { get; set; }
}

internal class BrushWithStateRenderInfo : IBrushWithState
{
    public IBrushProxy? NormalBrush { get; set; }
    public IBrushProxy? MouseOverBrush { get; set; }
    public IBrushProxy? PressedBrush { get; set; }
}

internal class PortStyleRenderInfo : IRenderInfo
{
    internal IBrushWithState? PortBrush { get; set; }
    internal IBrushProxy? TextBrush { get; set; }

    internal float PortRadius { get; set; } // TODO: figure out different port shapes

    internal TextPropertiesRenderInfo? TextProperties { get; set; }
}

internal class TextPropertiesRenderInfo : IRenderInfo
{
    internal string? FontFamily { get; set; }
    internal int FontWeight { get; set; }
    internal int Italic { get; set; }
    internal int FontSize { get; set; }
}

