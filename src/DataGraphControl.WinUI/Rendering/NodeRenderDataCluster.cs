using DataGraphControl.Core;
using DataGraphControl.WinUI.Acceleration;
using GhostCore.Data.Evaluation;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace DataGraphControl.WinUI.Rendering;
internal class NodeRenderDataBoundsSelector : IBoundsSelector<NodeRenderData>
{
    public Quad GetBounds(NodeRenderData data) => new(data.Rect.X, data.Rect.Y, 150, 150);
}

internal class NodeRenderDataCluster : Cluster<NodeRenderData, NodeRenderDataBoundsSelector>
{
    public NodeRenderDataCluster(Quad bounds, int id) : base(bounds, id)
    {
    }
}


public enum HitTestResultType
{
    None,
    NodeBody,
    PropertyText,
    PropertyPort,
    Title,
    Connection
}

public sealed partial record NodeRenderData(EvaluatableNode RenderData,
    Color BorderColor,
    float BorderThickness,
    Color Background,
    Rect Rect,
    Vector2 TitleLocation,
    CanvasTextFormat TextFormat,
    CanvasTextLayout TextLayout,
    float Radius,
    List<PropertyRenderData> InputProperties,
    List<PropertyRenderData> OutputProperties) : IDisposable
{
    public void Dispose()
    {
        TextFormat?.Dispose();
        TextLayout?.Dispose();
    }
}

public sealed partial record PropertyRenderData(PortInfo Data,
    Color Color,
    Vector2 CanvasPosition,
    Vector2 TextPosition,
    CanvasTextFormat TextFormat,
    CanvasTextLayout TextLayout,
    float Radius) : IDisposable
{
    public bool MouseOver { get; set; }

    public void Dispose()
    {
        TextFormat?.Dispose();
        TextLayout?.Dispose();
    }
}

public record ConnectionRenderData(Connection Data,
    float Thickness,
    Color Color,
    Vector2 Start,
    Vector2 End,
    Vector2 Control1,
    Vector2 Control2);