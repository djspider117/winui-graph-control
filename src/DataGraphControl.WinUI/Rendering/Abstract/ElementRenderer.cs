using DataGraphControl.WinUI.Rendering.Context;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace DataGraphControl.WinUI.Rendering.Abstract;

public abstract class ElementRenderer<TContext> : IElementRenderer
    where TContext : ElementRenderContext
{
    public void Render(ICanvasResourceCreator rc, CanvasAnimatedDrawEventArgs args, ref ElementRenderContext? context, ref GraphRenderState state, ref RenderInfoContext renderInfoContext)
    {
        if (context is not TContext ctx)
            return;

        if (ctx.Dirty)
        {
            LayoutInternal(rc, ref ctx, ref state, ref renderInfoContext);
            ctx.Dirty = false;
        }

        RenderInternal(rc, args, ref ctx, ref state, ref renderInfoContext);

        context = ctx;
    }

    public virtual void LayoutInternal(ICanvasResourceCreator rc, ref TContext ctx, ref GraphRenderState state, ref RenderInfoContext renderInfoContext) { }
    public abstract void RenderInternal(ICanvasResourceCreator rc, CanvasAnimatedDrawEventArgs args, ref TContext context, ref GraphRenderState state, ref RenderInfoContext renderInfoContext);
    public abstract ElementRenderContext CreateContext(ICanvasResourceCreator rc, object? data, ref GraphRenderState state, ref RenderInfoContext renderInfoContext);
}
