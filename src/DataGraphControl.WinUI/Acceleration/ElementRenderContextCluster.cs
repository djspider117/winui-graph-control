using DataGraphControl.Core;
using DataGraphControl.WinUI.Rendering.Context;

namespace DataGraphControl.WinUI.Acceleration;

public class ElementRenderContextCluster(Quad bounds, int id) : Cluster<ElementRenderContext, ElementBoundsSelector>(bounds, id);

public class ElementBoundsSelector : IBoundsSelector<ElementRenderContext>
{
    public Quad GetBounds(ElementRenderContext data) => new(data.Position, data.Size);
}


public class NodeCluster(Quad bounds, int id) : Cluster<INode, NodeBoundsSelector>(bounds, id);
public class NodeBoundsSelector : IBoundsSelector<INode>
{
    public Quad GetBounds(INode data) => new(data.Position.X, data.Position.Y, 8, 8);
}
