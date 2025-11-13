using DataGraphControl.WinUI.Acceleration;

namespace DataGraphControl.WinUI.Rendering;

public class ElementRenderContextCluster(Quad bounds, int id) : Cluster<ElementRenderContext, ElementBoundsSelector>(bounds, id);

public class ElementBoundsSelector : IBoundsSelector<ElementRenderContext>
{
    public Quad GetBounds(ElementRenderContext data) => new(data.Position, data.Size);
}
