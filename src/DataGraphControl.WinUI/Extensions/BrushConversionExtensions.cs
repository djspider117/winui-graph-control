using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DataGraphControl.WinUI.Extensions;

internal static class BrushConversionExtensions
{
    public static IBrushProxy ToBrushProxy(this Brush brush)
    {
        if (brush is SolidColorBrush scb)
            return scb.ToProxy();

        if (brush is LinearGradientBrush lgb)
            return lgb.ToProxy();

        if (brush is RadialGradientBrush rgb)
            return rgb.ToProxy();

        throw new InvalidOperationException($"Only the following brush types are supported: {nameof(SolidColorBrush)}, {nameof(LinearGradientBrush)}, {nameof(RadialGradientBrush)}");
    }

    internal static SolidColorBrushProxy ToProxy(this SolidColorBrush brush)
    {
        return new SolidColorBrushProxy { Color = brush.Color.ToVector4()};
    }

    internal static LinearColorBrushProxy ToProxy(this LinearGradientBrush brush)
    {
        return new LinearColorBrushProxy
        {
            GradientStops = brush.GradientStops.ToGradienStopProxyCollection(),
            StartPoint = brush.StartPoint.ToVector2(),
            EndPoint = brush.EndPoint.ToVector2()
        };
    }

    internal static RadialColorBrushProxy ToProxy(this RadialGradientBrush brush)
    {
        return new RadialColorBrushProxy
        {
            GradientStops = brush.GradientStops.ToGradienStopProxyCollection(),
            Center = brush.Center.ToVector2(),
            GradientOrigin = brush.GradientOrigin.ToVector2(),
            Radius = new Vector2((float)brush.RadiusX, (float)brush.RadiusY)
        };
    }

    public static Vector4 ToVector4(this Windows.UI.Color c) => new(c.A, c.R, c.G, c.B);

    internal static List<GradientStopProxy> ToGradienStopProxyCollection(this IEnumerable<GradientStop> gradientStops)
    {
        var rv = new List<GradientStopProxy>();
        foreach (var stop in gradientStops)
        {
            rv.Add(new() { Offset = (float)stop.Offset, Color = stop.Color.ToVector4() });
        }

        return rv;
    }
}
