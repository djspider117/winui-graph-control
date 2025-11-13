using DataGraphControl.WinUI.Rendering.Context;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace DataGraphControl.WinUI.Rendering.Abstract;

public interface IElementRenderer
{
    ElementRenderContext CreateContext(ICanvasResourceCreator rc, object? data, ref GraphRenderState state, ref RenderInfoContext renderInfoContext);
    void Render(ICanvasResourceCreator rc, CanvasAnimatedDrawEventArgs args, ref ElementRenderContext? context, ref GraphRenderState state, ref RenderInfoContext renderInfoContext);
}
