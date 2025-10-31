using GraphData;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace GraphSandbox;

public sealed partial class GraphRenderer : UserControl
{
    private readonly Graph _graph;
    private readonly GraphRenderState _state;

    public GraphRenderer()
    {
        _graph = DemoData.CreateDemoGraph();
        _state = new GraphRenderState(_graph);
        InitializeComponent();
        Loaded += GraphRenderer_Loaded;
    }

    private void GraphRenderer_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= GraphRenderer_Loaded;

    }

    private void NodeCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        _state.Invalidate(sender);
    }

    private void NodeCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (!_state.IsRenderable)
            return;

        var ds = args.DrawingSession;
        foreach ((var _, var node) in _state.Nodes)
        {
            ds.FillRoundedRectangle(node.Rect, node.Radius, node.Radius, node.Background);
            ds.DrawRoundedRectangle(node.Rect, node.Radius, node.Radius, node.BorderColor, node.BorderThickness);
            ds.DrawTextLayout(node.TextLayout, node.TitleLocation, Colors.White);

            RenderProperties(node.InputProperties, ds);
            RenderProperties(node.OutputProperties, ds);
        }

        foreach (var conn in _state.Connections)
        {
            var builder = new CanvasPathBuilder(sender);
            builder.BeginFigure(conn.Start);
            builder.AddCubicBezier(conn.Control1, conn.Control2, conn.End);
            builder.EndFigure(CanvasFigureLoop.Open);

            var geom = CanvasGeometry.CreatePath(builder);
            ds.DrawGeometry(geom, conn.Color, conn.Thickness);
        }
    }
    private static void RenderProperties(List<PropertyRenderData> propData, CanvasDrawingSession ds)
    {
        foreach (var prop in propData)
        {
            ds.FillCircle(prop.CanvasPosition, prop.Radius, Colors.White);
            ds.DrawTextLayout(prop.TextLayout, prop.TextPosition, Colors.White);
        }
    }
}

public class GraphRenderState
{
    private Graph _graph;

    public Dictionary<uint, NodeRenderData> Nodes { get; set; } = [];
    public List<ConnectionRenderData> Connections { get; set; } = [];

    public bool IsRenderable { get; private set; }

    public GraphRenderState(Graph graph)
    {
        _graph = graph;
    }

    public void Cleanup()
    {
        IsRenderable = false;
        foreach ((var _, var value) in Nodes)
        {
            foreach (var prop in value.InputProperties)
            {
                prop.Dispose();
            }

            foreach (var prop in value.OutputProperties)
            {
                prop.Dispose();
            }

            value.Dispose();
        }
        Nodes.Clear();
    }

    public void Invalidate(ICanvasResourceCreator rc)
    {
        Cleanup();
        foreach (var node in _graph.Nodes)
        {
            const int nodeWidth = 150;
            const int nodeHeight = 150;
            const int rowHeight = 26;

            // TODO: dispose these
            var textFormat12 = new CanvasTextFormat()
            {
                FontFamily = "Segoe UI",
                FontSize = 12,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            var textFormat10 = new CanvasTextFormat()
            {
                FontFamily = "Segoe UI",
                FontSize = 10,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            var layout = new CanvasTextLayout(rc, node.Name, textFormat12, float.MaxValue, float.MaxValue);

            var nrd = new NodeRenderData(node, Colors.LightGray, 2, Colors.Gray,
                new Rect(node.Position.X, node.Position.Y, nodeWidth, nodeHeight),
                new Vector2(nodeWidth / 2 - (float)layout.LayoutBounds.Width / 2 + node.Position.X, 10 + node.Position.Y),
                textFormat12,
                layout,
                8,
                [], []);

            Nodes.Add(node.Id, nrd);

            var offsetX = 15;
            var offsetY = (float)layout.LayoutBounds.Height + 10;
            int i = 0;
            foreach (var prop in node.InputProperties)
            {
                var centerRow = rowHeight / 2 + offsetY + i * rowHeight + node.Position.Y;

                layout = new CanvasTextLayout(rc, prop.Name, textFormat10, float.MaxValue, float.MaxValue);
                nrd.InputProperties.Add(new PropertyRenderData(
                    prop,
                    Colors.Gray,
                    new Vector2(offsetX + node.Position.X, centerRow),
                    new Vector2(offsetX + 10 + node.Position.X, centerRow - (float)layout.LayoutBounds.Height / 2),
                    textFormat10,
                    layout,
                    5));

                i++;
            }

            offsetX = nodeWidth - offsetX;
            i = 0;
            foreach (var prop in node.OutputProperties)
            {
                var centerRow = rowHeight / 2 + offsetY + i * rowHeight + node.Position.Y;

                layout = new CanvasTextLayout(rc, prop.Name, textFormat10, float.MaxValue, float.MaxValue);
                
                nrd.OutputProperties.Add(new PropertyRenderData(
                   prop,
                   Colors.Gray,
                   new Vector2(offsetX + node.Position.X, centerRow),
                   new Vector2(offsetX - 10 + node.Position.X - (float)layout.LayoutBounds.Width, centerRow - (float)layout.LayoutBounds.Height / 2),
                   textFormat10,
                   layout,
                   5));

                i++;
            }
        }

        foreach (var conn in _graph.Connections)
        {
            var srcNodeData = Nodes[conn.SourceNodeId];
            var targetNodeData = Nodes[conn.TargetNodeId];

            // TODO make properties dictionaries
            var srcProp = srcNodeData.OutputProperties.First(x => x.Data.PropertyId == conn.SourcePropertyId);
            var dstProp = targetNodeData.InputProperties.First(x => x.Data.PropertyId == conn.TargetPropertyId);

            Connections.Add(new ConnectionRenderData(conn,
                2, Colors.Black,
                srcProp.CanvasPosition,
                dstProp.CanvasPosition,
                new Vector2(srcProp.CanvasPosition.X + 32, srcProp.CanvasPosition.Y),
                new Vector2(dstProp.CanvasPosition.X - 32, dstProp.CanvasPosition.Y)));
        }
        IsRenderable = true;
    }
}

public sealed partial record NodeRenderData(Node Data,
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

public sealed partial record PropertyRenderData(NodeProperty Data, 
    Color Color, 
    Vector2 CanvasPosition,
    Vector2 TextPosition,
    CanvasTextFormat TextFormat,
    CanvasTextLayout TextLayout,
    float Radius) : IDisposable
{
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