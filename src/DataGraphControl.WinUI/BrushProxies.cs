using System.Collections.Generic;
using System.Numerics;

namespace DataGraphControl.WinUI;

internal interface IBrushProxy { }

internal interface IBrushWithState
{
    IBrushProxy? NormalBrush { get; set; }
    IBrushProxy? MouseOverBrush { get; set; }
    IBrushProxy? PressedBrush { get; set; }
}

internal class SolidColorBrushProxy : IBrushProxy
{
    internal Vector4 Color { get; set; }
}
internal class LinearColorBrushProxy : IBrushProxy
{
    internal Vector2 StartPoint { get; set; }
    internal Vector2 EndPoint { get; set; }
    internal List<GradientStopProxy> GradientStops { get; set; } = [];
}
internal class RadialColorBrushProxy : IBrushProxy
{
    internal Vector2 Center { get; set; }
    internal Vector2 GradientOrigin { get; set; }
    internal Vector2 Radius { get; set; }

    // missing: interpolation space, mapping mode, spread method

    internal List<GradientStopProxy> GradientStops { get; set; } = [];
}

internal class GradientStopProxy
{
    internal Vector4 Color { get; set; }
    internal float Offset { get; set; }
}