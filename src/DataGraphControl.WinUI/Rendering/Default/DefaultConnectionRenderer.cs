using DataGraphControl.Core;
using DataGraphControl.WinUI.Rendering.Abstract;
using DataGraphControl.WinUI.Rendering.Context;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using System;
using System.Linq;
using System.Numerics;

namespace DataGraphControl.WinUI.Rendering.Default;

internal class DefaultConnectionRenderer : ElementRenderer<ConnectionRenderContext>
{
    public override void LayoutInternal(ICanvasResourceCreator rc, ref ConnectionRenderContext ctx, ref GraphRenderState state, ref RenderInfoContext renderInfoContext)
    {
        var conn = ctx.Data;

        var srcNodeData = state.NodeContextCache[conn.SourceNodeId] as NodeRenderContext;
        var targetNodeData = state.NodeContextCache[conn.TargetNodeId] as NodeRenderContext;

        // TODO make properties dictionaries
        var srcProp = srcNodeData!.OutputProperties.First(x => x.Id == conn.SourcePropertyId);
        var dstProp = targetNodeData!.InputProperties.First(x => x.Id == conn.TargetPropertyId);

        ctx.Thickness = 2; // TODO style
        ctx.Color = Colors.White; // TODO style

        ctx.Start = srcProp.PortPosition;
        ctx.Control1 = new Vector2(srcProp.PortPosition.X + 32, srcProp.PortPosition.Y);
        ctx.Control2 = new Vector2(dstProp.PortPosition.X - 32, dstProp.PortPosition.Y);
        ctx.End = dstProp.PortPosition;
    }

    public override void RenderInternal(ICanvasResourceCreator rc, CanvasAnimatedDrawEventArgs args, ref ConnectionRenderContext context, ref GraphRenderState state, ref RenderInfoContext renderInfoContext)
    {
        var ds = args.DrawingSession;

        var builder = new CanvasPathBuilder(rc);
        builder.BeginFigure(context.Start);
        builder.AddCubicBezier(context.Control1, context.Control2, context.End - new Vector2(14, 0));
        builder.EndFigure(CanvasFigureLoop.Open);

        var geom = CanvasGeometry.CreatePath(builder);
        ds.DrawGeometry(geom, context.Color, context.Thickness);

        Vector2 tangent = 3 * (context.Control2 - context.End);
        if (tangent.LengthSquared() < 1e-6f)
            tangent = Vector2.Normalize(context.End - context.Start); // fallback if degenerate
        else
            tangent = Vector2.Normalize(tangent);

        float arrowLength = 15f;
        float arrowAngle = 25f * (float)(Math.PI / 180.0); // 25 degrees

        var a = Matrix3x2.CreateRotation(arrowAngle);
        var b = Matrix3x2.CreateRotation(-arrowAngle);

        Vector2 right = Vector2.Transform(tangent, a) * arrowLength;
        Vector2 left = Vector2.Transform(tangent, b) * arrowLength;

        Vector2 tip = context.End;
        Vector2 pLeft = tip + left;
        Vector2 pRight = tip + right;

        ds.FillGeometry(
            CanvasGeometry.CreatePolygon(ds, new[] { tip, pLeft, pRight }),
            Colors.White
        );
    }
    public override ElementRenderContext CreateContext(ICanvasResourceCreator rc, object? data, ref GraphRenderState state, ref RenderInfoContext renderInfoContext)
    {
        if (data == null)
            return ElementRenderContext.Empty;

        return new ConnectionRenderContext((Connection)data, 2, Colors.White, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero);
    }
}
