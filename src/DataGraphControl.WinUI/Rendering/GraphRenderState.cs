using DataGraphControl.Core;
using DataGraphControl.WinUI.Acceleration;
using GhostCore.Data.Evaluation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation;
using Windows.UI;

namespace DataGraphControl.WinUI.Rendering;
public class GraphRenderState
{
    private IGraph? _graph;
    private NodeRenderDataCluster _cluster;

    public Dictionary<uint, NodeRenderData> Nodes { get; set; } = [];
    public List<ConnectionRenderData> Connections { get; set; } = [];

    public bool IsRenderable { get; private set; }

    public GraphRenderState(IGraph? graph)
    {
        _graph = graph;
        _cluster = new NodeRenderDataCluster(new Quad(-200, -200, 2000, 2000), 99999);
    }

    public void ResetGraph(IGraph? graph)
    {
        Cleanup();
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
        Connections.Clear();
        _cluster.Clear();
    }

    public void Invalidate(ICanvasResourceCreator rc)
    {
        Cleanup();

        if (_graph == null)
            return;

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
            _cluster.Insert(nrd);

            var offsetX = 8;
            var offsetY = (float)layout.LayoutBounds.Height + 10;
            int i = 0;
            foreach (var prop in node.TypeDefinition.Inputs)
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
            foreach (var prop in node.TypeDefinition.Outputs)
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
            var srcProp = srcNodeData.OutputProperties.First(x => x.Data.Id == conn.SourcePropertyId);
            var dstProp = targetNodeData.InputProperties.First(x => x.Data.Id == conn.TargetPropertyId);

            Connections.Add(new ConnectionRenderData(conn,
                2, Colors.White,
                srcProp.CanvasPosition,
                dstProp.CanvasPosition,
                new Vector2(srcProp.CanvasPosition.X + 32, srcProp.CanvasPosition.Y),
                new Vector2(dstProp.CanvasPosition.X - 32, dstProp.CanvasPosition.Y)));
        }
        IsRenderable = true;
    }

    public object? HitTest(Point position, out HitTestResultType resultType)
    {
        resultType = HitTestResultType.None;

        var p = position.ToVector2();

        foreach (var item in _cluster.Query(position))
        {
            if (item.Rect.Contains(position))
            {
                foreach (var iprop in item.InputProperties)
                {
                    var dst = Vector2.DistanceSquared(iprop.CanvasPosition, p);
                    if (dst <= MathF.Pow(iprop.Radius, 2))
                    {
                        resultType = HitTestResultType.PropertyPort;
                        return iprop;
                    }
                }

                foreach (var oprop in item.OutputProperties)
                {
                    var dst = Vector2.DistanceSquared(oprop.CanvasPosition, p);
                    if (dst <= MathF.Pow(oprop.Radius, 2))
                    {
                        resultType = HitTestResultType.PropertyPort;
                        return oprop;
                    }
                }

                resultType = HitTestResultType.NodeBody;
                return item;
            }
        }

        return null;
    }
}