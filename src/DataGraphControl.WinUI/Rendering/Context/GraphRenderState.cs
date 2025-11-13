using DataGraphControl.Core;
using DataGraphControl.WinUI.Acceleration;
using GhostCore.Data.Evaluation;
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

namespace DataGraphControl.WinUI.Rendering.Context;

public readonly struct GraphRenderState : IDisposable
{
    public readonly IGraph? Graph;
    public readonly ElementRenderContextCluster Cluster;
    public readonly NodeCluster RestrictedCluster;
    public readonly Dictionary<uint, ElementRenderContext> NodeContextCache = [];
    public readonly Dictionary<Connection, ElementRenderContext> ConnectionContextCache = [];

    public GraphRenderState(IGraph graph, ElementRenderContextCluster cluster)
    {
        Graph = graph;
        Cluster = cluster;

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (var node in Graph.Nodes)
        {
            var x = node.Position.X;
            var y = node.Position.Y;

            minX = float.Min(minX, x);
            minY = float.Min(minY, y);
            maxX = float.Max(maxX, x);
            maxY = float.Max(maxY, y);
        }

        minX = float.Min(minX, 0);
        minY = float.Min(minY, 0);
        maxX += 1000;
        maxY += 1000;

        RestrictedCluster = new(new Quad(minX, minY, maxX - minX, maxY - minY), int.MaxValue);
        foreach (var node in Graph.Nodes)
        {
            RestrictedCluster.Insert(node);
        }
    }

    public void Dispose()
    {
        Clean();
    }

    public void Clean()
    {
        if (NodeContextCache != null)
        {
            foreach (var item in NodeContextCache)
                item.Value.Dispose();
        }

        if (ConnectionContextCache != null)
        {
            foreach (var item in ConnectionContextCache)
                item.Value.Dispose();
        }

        NodeContextCache?.Clear();
        ConnectionContextCache?.Clear();
        Cluster?.Clear();
    }
}
