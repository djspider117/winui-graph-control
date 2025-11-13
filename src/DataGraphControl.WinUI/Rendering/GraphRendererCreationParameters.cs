using DataGraphControl.WinUI.Rendering.Abstract;
using DataGraphControl.WinUI.Rendering.Default;
using System.Collections.Generic;

namespace DataGraphControl.WinUI.Rendering;

public class GraphRendererCreationParameters
{
    public Dictionary<uint, IElementRenderer> NodeRenders { get; } = [];
    public IElementRenderer DefaultNodeRenderer { get; }
    public IElementRenderer ConnectionRenderer { get; }

    public GraphRendererCreationParameters(IElementRenderer? nodeRenderer = null, IElementRenderer? connectionRenderer = null)
    {
        DefaultNodeRenderer = nodeRenderer ?? new DefaultNodeRenderer();
        ConnectionRenderer = connectionRenderer ?? new DefaultConnectionRenderer();
    }
}