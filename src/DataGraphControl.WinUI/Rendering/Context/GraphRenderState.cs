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

public readonly struct GraphRenderState(IGraph graph, ElementRenderContextCluster cluster) : IDisposable
{
    public readonly IGraph? Graph = graph;
    public readonly ElementRenderContextCluster Cluster = cluster;
    public readonly Dictionary<uint, ElementRenderContext> NodeContextCache = [];
    public readonly Dictionary<Connection, ElementRenderContext> ConnectionContextCache = [];

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
