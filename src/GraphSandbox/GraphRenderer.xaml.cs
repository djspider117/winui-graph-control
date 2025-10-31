using GraphData;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

namespace GraphSandbox;

public sealed partial class GraphRenderer : UserControl
{
    private Compositor? _compositor;
    private ContainerVisual? _rootVisual;

    public GraphRenderer()
    {
        InitializeComponent();
        Loaded += GraphRenderer_Loaded;
    }

    private void GraphRenderer_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= GraphRenderer_Loaded;

        var graph = DemoData.CreateDemoGraph();

        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        _rootVisual = _compositor.CreateContainerVisual();

        foreach (var node in graph.Nodes)
        {
            const int nodeWidth = 150;
            const int nodeHeight = 200;

            var sprite = _compositor.CreateSpriteVisual();
            sprite.Offset = new Vector3(node.Position, 0);
            sprite.Brush = _compositor.CreateColorBrush(Colors.Gray);
            sprite.Size = new Vector2(nodeWidth, nodeHeight);

            var ds = _compositor.CreateDropShadow();
            sprite.Shadow = ds;

            RenderProperties(node.InputProperties, sprite, 5, 35);
            RenderProperties(node.OutputProperties, sprite, nodeWidth - 20, 35);

            _rootVisual.Children.InsertAtTop(sprite);
        }

        ElementCompositionPreview.SetElementChildVisual(MainGrid, _rootVisual);
    }

    public void RenderProperties(IEnumerable<NodeProperty> props, ContainerVisual parent, float offsetx, float offsety = 10)
    {
        int i = 0;
        foreach (var property in props)
        {
            var circleGeom = _compositor!.CreateEllipseGeometry();
            circleGeom.Radius = new Vector2(15);

            var propVisual = _compositor!.CreateShapeVisual();
            propVisual.Shapes.Add(_compositor!.CreateSpriteShape(circleGeom));

            var sprite = _compositor!.CreateSpriteVisual();
            sprite.Brush = _compositor.CreateColorBrush(Colors.LightGray);
            sprite.Size = new Vector2(15, 15);
            sprite.Offset = new Vector3(offsetx, i * 25 + offsety, 0);

            parent.Children.InsertAtTop(sprite);
            i++;
        }
    }
}
