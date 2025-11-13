using DataGraphControl.Core;
using DataGraphControl.WinUI.Extensions;
using DataGraphControl.WinUI.Rendering.Abstract;
using DataGraphControl.WinUI.Rendering.Context;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace DataGraphControl.WinUI.Rendering.Default;

internal class DefaultNodeRenderer : ElementRenderer<NodeRenderContext>
{
    public override ElementRenderContext CreateContext(ICanvasResourceCreator rc, object? data, ref GraphRenderState state, ref RenderInfoContext renderInfoContext)
    {
        if (data is not INode node)
            return ElementRenderContext.Empty;

        var rv = new NodeRenderContext(node, renderInfoContext.NodeStyleRenderInfo);
        LayoutInternal(rc, ref rv, ref state, ref renderInfoContext);
        return rv;
    }

    public override void LayoutInternal(ICanvasResourceCreator rc, ref NodeRenderContext ctx, ref GraphRenderState state, ref RenderInfoContext renderInfoContext)
    {
        var node = ctx.Data;
        var style = ctx.Style;
        var rowHeight = style.RowHeight;

        ctx.BorderStateBrush = new(style.BorderBrush!.ToCanvasBrush(rc), style.BorderBrush?.ToCanvasBrush(rc));
        ctx.BackgroundBrush = style.BackgroundBrush?.ToCanvasBrush(rc);
        ctx.TitleBrush = style.TitleBrush?.ToCanvasBrush(rc);
        ctx.TitleLayout = new CanvasTextLayout(rc, node.Name, ctx.TitleFormat, float.MaxValue, float.MaxValue);

        var textFormat10 = new CanvasTextFormat()
        {
            FontFamily = ctx.Style.TitleTextProperties?.FontFamily ?? "Segoe UI",
            FontSize = 10,
            WordWrapping = CanvasWordWrapping.NoWrap
        };
        ctx.DisposableResources.Add(textFormat10);

        ctx.Position = node.Position;
        ctx.Size = new Vector2(style.MinMaxWidth.X, style.MinMaxHeight.X); // TODO layout
        ctx.TitlePosition = new Vector2(ctx.Size.X / 2 - (float)ctx.TitleLayout.LayoutBounds.Width / 2 + ctx.Position.X, 10 + ctx.Position.Y);

        var offsetX = 8.0f;
        var offsetY = (float)ctx.TitleLayout.LayoutBounds.Height + 10;
        int i = 0;
        foreach (var prop in node.TypeDefinition.Inputs)
        {
            var centerRow = rowHeight / 2 + offsetY + i * rowHeight + node.Position.Y;

            var layout = new CanvasTextLayout(rc, prop.Name, textFormat10, float.MaxValue, float.MaxValue);
            ctx.InputProperties.Add(new PropertyRenderContext()
            {
                Id = prop.Id,
                TextLayout = layout,
                TextFormat = textFormat10,
                PortPosition = new Vector2(offsetX + node.Position.X, centerRow),
                Position = new Vector2(offsetX + 10 + node.Position.X, centerRow - (float)layout.LayoutBounds.Height / 2),
                PortBrush = new(style.InputPortStyle!.PortBrush!.NormalBrush!.ToCanvasBrush(rc), style.InputPortStyle!.PortBrush!.PressedBrush!.ToCanvasBrush(rc)),
                TextBrush = style.InputPortStyle!.TextBrush!.ToCanvasBrush(rc),
                Radius = style.InputPortStyle!.PortRadius
            });

            i++;
        }

        offsetX = ctx.Size.X - offsetX;
        i = 0;
        foreach (var prop in node.TypeDefinition.Outputs)
        {
            var centerRow = rowHeight / 2 + offsetY + i * rowHeight + node.Position.Y;

            var layout = new CanvasTextLayout(rc, prop.Name, textFormat10, float.MaxValue, float.MaxValue);

            var localStyle = style.OutputPortStyle!;

            ctx.OutputProperties.Add(new PropertyRenderContext()
            {
                Id = prop.Id,
                TextLayout = layout,
                TextFormat = textFormat10,
                PortPosition = new Vector2(offsetX + node.Position.X, centerRow),
                Position = new Vector2(offsetX - 10 + node.Position.X - (float)layout.LayoutBounds.Width, centerRow - (float)layout.LayoutBounds.Height / 2),
                PortBrush = new(localStyle.PortBrush!.NormalBrush!.ToCanvasBrush(rc), localStyle.PortBrush!.PressedBrush!.ToCanvasBrush(rc)),
                TextBrush = localStyle.TextBrush!.ToCanvasBrush(rc),
                Radius = localStyle.PortRadius
            });

            i++;
        }
    }

    public override void RenderInternal(ICanvasResourceCreator rc, CanvasAnimatedDrawEventArgs args, ref NodeRenderContext node, ref GraphRenderState state, ref RenderInfoContext renderInfoContext)
    {
        var ds = args.DrawingSession;
        if (node.Style.CornerRadius != Vector2.Zero)
        {
            var cr = node.Style.CornerRadius;
            ds.FillRoundedRectangle(node.Rect, cr.X, cr.Y, node.BackgroundBrush);

            var brush = node.VisualState.IsSelected ? node.BorderStateBrush?.SelectedBrush : node.BorderStateBrush?.NormalBrush;
            if (brush != null)
                ds.DrawRoundedRectangle(node.Rect, cr.X, cr.Y, brush);
        }
        else
        {
            ds.FillRectangle(node.Rect, node.BackgroundBrush);
        }

        ds.DrawTextLayout(node.TitleLayout, node.TitlePosition, node.TitleBrush);

        RenderProperties(node.InputProperties, ds);
        RenderProperties(node.OutputProperties, ds);
    }

    private static void RenderProperties(List<PropertyRenderContext> propData, CanvasDrawingSession ds)
    {
        foreach (var prop in propData)
        {
            var brush = prop.VisualState.IsSelected ? prop.PortBrush?.SelectedBrush : prop.PortBrush?.NormalBrush;
            if (brush == null)
                continue;

            ds.FillCircle(prop.PortPosition, prop.Radius, brush);
            ds.DrawTextLayout(prop.TextLayout, prop.Position, prop.PortBrush?.NormalBrush);
        }
    }
}
