using CommunityToolkit.WinUI;
using DataGraphControl.Core;
using DataGraphControl.WinUI.Rendering;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DataGraphControl.WinUI;
public sealed partial class DataGraphView : UserControl
{
    private NodeStyleRenderInfo? _nodeStyleRenderInfo;
    private GraphRenderState? _renderState;
    private volatile bool _invaldated = false;

    [GeneratedDependencyProperty]
    public partial IGraph? GraphData { get; set; }
    partial void OnGraphDataChanged(IGraph? newValue)
    {
        if (newValue == null)
            return;

        _renderState?.ResetGraph(newValue);
        _invaldated = true;
    }

    [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetNodeStyleDefinitionDefault))]
    public partial NodeStyleDefinition NodeStyleDefinition { get; set; }
    private static NodeStyleDefinition GetNodeStyleDefinitionDefault() => NodeStyleDefinition.Default;
    partial void OnNodeStyleDefinitionPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is NodeStyleDefinition old)
            old.InternalPropertyChanged -= NodeStyleDefinition_InternalPropertyChanged;

        if (e.NewValue is NodeStyleDefinition nw)
            nw.InternalPropertyChanged += NodeStyleDefinition_InternalPropertyChanged;
    }

    private void NodeStyleDefinition_InternalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (NodeStyleDefinition == null)
            return;

        _nodeStyleRenderInfo = NodeStyleDefinition.GetRenderInfo();
        _invaldated = true;
    }

    public DataGraphView()
    {
        InitializeComponent();
        Loaded += DataGraphView_Loaded;
        Unloaded += DataGraphView_Unloaded;
    }

    private void DataGraphView_Loaded(object sender, RoutedEventArgs e)
    {
        _renderState = new(GraphData);
    }

    private void DataGraphView_Unloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= DataGraphView_Loaded;
        NodeCanvas.Draw -= NodeCanvas_Draw;
        NodeCanvas.CreateResources -= NodeCanvas_CreateResources;

    }

    private void NodeCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_invaldated)
        {
            RecreateResources(sender);
            _invaldated = false;
        }

        RenderGraph(sender, args.DrawingSession);
    }
    private void NodeCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        RecreateResources(sender);
    }

    private void RecreateResources(ICanvasResourceCreator resourceCreator)
    {
        if (_renderState == null)
            _invaldated = true;
        else
            _renderState.Invalidate(resourceCreator);
    }

    private void RenderGraph(ICanvasAnimatedControl sender, CanvasDrawingSession ds)
    {
        if (_renderState == null)
            return;

        if (!_renderState.IsRenderable)
            return;

        foreach ((var _, var node) in _renderState.Nodes)
        {
            ds.FillRoundedRectangle(node.Rect, node.Radius, node.Radius, node.Background);
            //if (node.RenderData.Selected)
            //    ds.DrawRoundedRectangle(node.Rect, node.Radius, node.Radius, Colors.Red, node.BorderThickness);
            //else
                ds.DrawRoundedRectangle(node.Rect, node.Radius, node.Radius, node.BorderColor, node.BorderThickness);
            ds.DrawTextLayout(node.TextLayout, node.TitleLocation, Colors.White);

            RenderProperties(node.InputProperties, ds);
            RenderProperties(node.OutputProperties, ds);
        }

        foreach (var conn in _renderState.Connections)
        {
            RenderConnection(sender, ds, conn);
        }
    }

    private static void RenderConnection(ICanvasAnimatedControl sender, CanvasDrawingSession ds, ConnectionRenderData conn)
    {
        var builder = new CanvasPathBuilder(sender);
        builder.BeginFigure(conn.Start);
        builder.AddCubicBezier(conn.Control1, conn.Control2, conn.End - new Vector2(14, 0));
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

}
