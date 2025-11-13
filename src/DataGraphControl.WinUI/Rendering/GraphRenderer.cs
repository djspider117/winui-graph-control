using DataGraphControl.Core;
using DataGraphControl.WinUI.Acceleration;
using DataGraphControl.WinUI.Rendering.Abstract;
using DataGraphControl.WinUI.Rendering.Context;
using DataGraphControl.WinUI.Rendering.Default;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DataGraphControl.WinUI.Rendering;

internal class GraphRenderer
{
    private readonly IDictionary<uint, IElementRenderer> _nodeRenders;
    private readonly IElementRenderer _defaultNodeRenderer;
    private readonly IElementRenderer _connectionRenderer;
    private RenderInfoContext _renderInfoContext = new();
    private GraphRenderState _state;

    public NodeStyleRenderInfo? NodeStyleRenderInfo
    {
        get => _renderInfoContext.NodeStyleRenderInfo;
        set => _renderInfoContext.NodeStyleRenderInfo = value;
    }

    public GraphRenderer(IGraph? graph, GraphRendererCreationParameters creationParams)
    {
        _nodeRenders = creationParams.NodeRenders.ToFrozenDictionary();
        _defaultNodeRenderer = creationParams.DefaultNodeRenderer;
        _connectionRenderer = creationParams.ConnectionRenderer;

        if (graph != null)
            _state = new GraphRenderState(graph, GetClusterForGraph(graph));

        NodeStyleRenderInfo = NodeStyleDefinition.Default.GetRenderInfo();
    }

    public void ResetGraph(IGraph graph)
    {
        _state.Dispose();
        _state = new GraphRenderState(graph, GetClusterForGraph(graph));
    }
    public void Clean()
    {
        _state.Clean();
    }

    public void Invalidate(ICanvasResourceCreator rc)
    {
        foreach (var node in _state.Graph?.Nodes ?? [])
        {
            var typeId = node.TypeDefinition.TypeId;
            var nodeRenderer = _nodeRenders.TryGetValue(typeId, out var renderer) ? renderer : _defaultNodeRenderer;

            CacheElementState(nodeRenderer, rc, _state.NodeContextCache, node.Id, node);
        }

        foreach (var conn in _state.Graph?.Connections ?? [])
        {
            CacheElementState(_connectionRenderer, rc, _state.ConnectionContextCache, conn, conn);
        }
    }

    public void Render(ICanvasResourceCreator rc, CanvasAnimatedDrawEventArgs args)
    {
        foreach (var node in _state.Graph?.Nodes ?? [])
        {
            var typeId = node.TypeDefinition.TypeId;
            var nodeRenderer = _nodeRenders.TryGetValue(typeId, out var renderer) ? renderer : _defaultNodeRenderer;

            RenderElement(nodeRenderer, rc, args, _state.NodeContextCache, node.Id, node, ref _state);
        }

        foreach (var conn in _state.Graph?.Connections ?? [])
        {
            RenderElement(_connectionRenderer, rc, args, _state.ConnectionContextCache, conn, conn, ref _state);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RenderElement<TKey, TData>(IElementRenderer renderer, ICanvasResourceCreator rc, CanvasAnimatedDrawEventArgs args, Dictionary<TKey, ElementRenderContext> cache, TKey key, TData? data, ref GraphRenderState state)
        where TKey : notnull
    {
        ElementRenderContext? context = CacheElementState(renderer, rc, cache, key, data);
        renderer.Render(rc, args, ref context, ref state, ref _renderInfoContext);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ElementRenderContext CacheElementState<TKey, TData>(IElementRenderer renderer, ICanvasResourceCreator rc, Dictionary<TKey, ElementRenderContext> cache, TKey key, TData? data) 
        where TKey : notnull
    {
        if (!cache.TryGetValue(key, out var context))
        {
            context = renderer.CreateContext(rc, data, ref _state, ref _renderInfoContext);
            cache.Add(key, context);
        }

        return context;
    }

    protected virtual ElementRenderContextCluster GetClusterForGraph(IGraph graph) => new(new(-200, -200, 2000, 2000), 99999);
}
