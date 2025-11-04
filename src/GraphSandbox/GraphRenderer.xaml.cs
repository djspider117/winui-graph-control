using GraphData;
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

        if (_requestInvalidate)
        {
            _state.Invalidate(sender);
            _requestInvalidate = false;
        }

        foreach ((var _, var node) in _state.Nodes)
        {
            ds.FillRoundedRectangle(node.Rect, node.Radius, node.Radius, node.Background);
            if (node.Data.Selected)
                ds.DrawRoundedRectangle(node.Rect, node.Radius, node.Radius, Colors.Red, node.BorderThickness);
            else
                ds.DrawRoundedRectangle(node.Rect, node.Radius, node.Radius, node.BorderColor, node.BorderThickness);
            ds.DrawTextLayout(node.TextLayout, node.TitleLocation, Colors.White);

            RenderProperties(node.InputProperties, ds);
            RenderProperties(node.OutputProperties, ds);
        }

        foreach (var conn in _state.Connections)
        {
            RenderConnection(sender, ds, conn);
        }
    }

    private static void RenderConnection(ICanvasAnimatedControl sender, CanvasDrawingSession ds, ConnectionRenderData conn)
    {
        var builder = new CanvasPathBuilder(sender);
        builder.BeginFigure(conn.Start);
        builder.AddCubicBezier(conn.Control1, conn.Control2, conn.End - new Vector2(14,0));
        builder.EndFigure(CanvasFigureLoop.Open);

        var geom = CanvasGeometry.CreatePath(builder);
        ds.DrawGeometry(geom, conn.Color, conn.Thickness);

        Vector2 tangent = 3 * (conn.Control2 - conn.End);
        if (tangent.LengthSquared() < 1e-6f)
            tangent = Vector2.Normalize(conn.End - conn.Start); // fallback if degenerate
        else
            tangent = Vector2.Normalize(tangent);

        float arrowLength = 15f;
        float arrowAngle = 25f * (float)(Math.PI / 180.0); // 25 degrees

        var a = Matrix3x2.CreateRotation(arrowAngle);
        var b = Matrix3x2.CreateRotation(-arrowAngle);

        Vector2 right = Vector2.Transform(tangent, a) * arrowLength;
        Vector2 left = Vector2.Transform(tangent, b) * arrowLength;

        Vector2 tip = conn.End;
        Vector2 pLeft = tip + left;
        Vector2 pRight = tip + right;

        ds.FillGeometry(
            CanvasGeometry.CreatePolygon(ds, new[] { tip, pLeft, pRight }),
            Colors.White
        );
    }

    private static void RenderProperties(List<PropertyRenderData> propData, CanvasDrawingSession ds)
    {
        foreach (var prop in propData)
        {
            ds.FillCircle(prop.CanvasPosition, prop.Radius, prop.MouseOver ? Colors.Red : Colors.White);
            ds.DrawTextLayout(prop.TextLayout, prop.TextPosition, Colors.White);
        }
    }

    private bool _pressed;
    private bool _moved;
    private Node? _selectedNode;
    private Point _startPoint;
    private Point _lastPoint;
    private bool _requestInvalidate;
    private NodeRenderData? _draggingState;
    private PropertyRenderData? _lastPort;
    //private object? _lastHitObject;
    //private HitTestResultType _lastHitType;

    public const float MAX_DISTANCE = 2;

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        _pressed = true;
        _startPoint = e.GetCurrentPoint(this).Position;
        _lastPoint = _startPoint;
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        base.OnPointerMoved(e);
        _moved = true;

        var curPt = e.GetCurrentPoint(this).Position;

        var htBox = _state.HitTest(curPt, out var type);

        if (_lastPort != null)
            _lastPort.MouseOver = false;

        if (type == HitTestResultType.PropertyPort)
        {
            _lastPort = htBox as PropertyRenderData;
            _lastPort!.MouseOver = true;
        }

        var distance = Vector2.Distance(_startPoint.ToVector2(), curPt.ToVector2());
        if (distance < MAX_DISTANCE)
            _moved = false;

        if (!_moved || !_pressed)
        {
            _lastPoint = curPt;
            return;
        }

        if (type == HitTestResultType.NodeBody)
        {
            if (_draggingState == null)
                _draggingState = htBox as NodeRenderData;

            if (_draggingState == null)
            {
                _lastPoint = curPt;
                return;
            }

            var delta = curPt.ToVector2() - _lastPoint.ToVector2();
            _draggingState.Data.Position += delta;

            _lastPoint = curPt;
            _requestInvalidate = true;
        }

    }

    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        base.OnPointerReleased(e);
        _draggingState = null;

        var distance = Vector2.Distance(_startPoint.ToVector2(), e.GetCurrentPoint(this).Position.ToVector2());
        if (distance < MAX_DISTANCE)
            _moved = false;

        if (_pressed && !_moved)
        {
            //CLICK
            _pressed = false;
            _moved = false;

            var pt = e.GetCurrentPoint(this).Position;
            var htBox = _state.HitTest(pt, out var type);

            if (type == HitTestResultType.NodeBody)
            {
                var ht = htBox as NodeRenderData;

                if (_selectedNode != null)
                    _selectedNode.Selected = false;

                if (ht == null)
                    return;

                ht.Data.Selected = true;
                _selectedNode = ht.Data;
                return;
            }
        }


        _pressed = false;
        _moved = false;
        // RELEASE AFTER DRAG
    }

    //todo add all the pointer events
}

public class GraphRenderState
{
    private Graph _graph;
    private QuadTree<NodeRenderData> _qt;


    private class NodeRenderDataBoundsSelector : IBoundsSelector<NodeRenderData>
    {
        public Vector4 GetBounds(NodeRenderData value) => new ((float)value.Rect.Left, (float)value.Rect.Top, (float)value.Rect.Right, (float)value.Rect.Bottom);
    }

    public Dictionary<uint, NodeRenderData> Nodes { get; set; } = [];
    public List<ConnectionRenderData> Connections { get; set; } = [];

    public bool IsRenderable { get; private set; }

    public GraphRenderState(Graph graph)
    {
        _graph = graph;
        _qt = new QuadTree<NodeRenderData>(-500f, -500f, 2000f, 2000f, new NodeRenderDataBoundsSelector(), 11111, 20);

        // TODO: implement hittesting with quadtree
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
        _qt.Clear();
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
            _qt.Insert(nrd);

            var offsetX = 8;
            var offsetY = (float)layout.LayoutBounds.Height + 10;
            int i = 0;
            foreach (var prop in node.TypeDefinition.InputProperties)
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
            foreach (var prop in node.TypeDefinition.OutputProperties)
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

        

        foreach (var item in Nodes.Values)
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

public enum HitTestResultType
{
    None,
    NodeBody,
    PropertyText,
    PropertyPort,
    Title,
    Connection
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