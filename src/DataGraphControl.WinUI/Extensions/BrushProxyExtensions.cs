using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;

namespace DataGraphControl.WinUI.Extensions;

internal static class BrushProxyExtensions
{
    public static ICanvasBrush ToCanvasBrush(this IBrushProxy proxy, ICanvasResourceCreator rc)
    {
        if (proxy is SolidColorBrushProxy scb)
            return scb.ToCanvasBrush(rc);

        if (proxy is LinearColorBrushProxy lcb)
            return lcb.ToCanvasBrush(rc);

        if (proxy is RadialColorBrushProxy rcb)
            return rcb.ToCanvasBrush(rc);

        throw new InvalidOperationException($"Unsupported {nameof(IBrushProxy)} implementation.");
    }

    private static CanvasSolidColorBrush ToCanvasBrush(this SolidColorBrushProxy scb, ICanvasResourceCreator rc) => new(rc, scb.Color.ToColor());
    private static CanvasLinearGradientBrush ToCanvasBrush(this LinearColorBrushProxy lcb, ICanvasResourceCreator rc)
    {
        var stops = new CanvasGradientStop[lcb.GradientStops.Count];

        for (int i = 0; i < lcb.GradientStops.Count; i++)
        {
            var gs = lcb.GradientStops[i];
            stops[i] = new(gs.Offset, gs.Color.ToColor());
        }
        return new CanvasLinearGradientBrush(rc, [.. stops])
        {
            StartPoint = lcb.StartPoint,
            EndPoint = lcb.EndPoint,
        };
    }
    private static CanvasRadialGradientBrush ToCanvasBrush(this RadialColorBrushProxy rcb, ICanvasResourceCreator rc)
    {
        var stops = new CanvasGradientStop[rcb.GradientStops.Count];

        for (int i = 0; i < rcb.GradientStops.Count; i++)
        {
            var gs = rcb.GradientStops[i];
            stops[i] = new(gs.Offset, gs.Color.ToColor());
        }
        return new CanvasRadialGradientBrush(rc, [.. stops])
        {
            RadiusX = rcb.Radius.X,
            RadiusY = rcb.Radius.Y,
            Center = rcb.Center,
            OriginOffset = rcb.GradientOrigin
        };
    }
}
