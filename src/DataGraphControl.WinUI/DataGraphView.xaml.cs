using CommunityToolkit.WinUI;
using DataGraphControl.Core;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace DataGraphControl.WinUI;
public sealed partial class DataGraphView : UserControl
{
    private NodeStyleRenderInfo? _nodeStyleRenderInfo;
    private volatile bool _invaldated = false;

    [GeneratedDependencyProperty]
    public partial IGraph? GraphData { get; set; }
    partial void OnGraphDataChanged(IGraph? newValue)
    {
        if (newValue == null)
            return;

        // TODO set graph to render state
        _invaldated = true;
    }

    [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetNodeStyleDefinitionDefault))]
    public partial NodeStyleDefinition NodeStyleDefinition { get; set; }
    private static NodeStyleDefinition GetNodeStyleDefinitionDefault() => NodeStyleDefinition.Default;
    partial void OnNodeStyleDefinitionPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is NodeStyleDefinition old)
            old.InternalPropertyChanged -= NodeStyleDefinition_InternalPropertyChanged;

        if (e.NewValue is NodeStyleDefinition nw)
            nw.InternalPropertyChanged += NodeStyleDefinition_InternalPropertyChanged;
    }

    private void NodeStyleDefinition_InternalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (NodeStyleDefinition == null)
            return;

        _nodeStyleRenderInfo = NodeStyleDefinition.GetRenderInfo();
        _invaldated = true;
    }

    public DataGraphView()
    {
        InitializeComponent();
        Unloaded += DataGraphView_Unloaded;
    }

    private void DataGraphView_Unloaded(object sender, RoutedEventArgs e)
    {
        NodeCanvas.Draw -= NodeCanvas_Draw;
        NodeCanvas.CreateResources -= NodeCanvas_CreateResources;
    }

    private void NodeCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_invaldated)
        {
            RecreateResources(sender);
            _invaldated = false;
        }

        RenderGraph(args.DrawingSession);
    }

    private void NodeCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        RecreateResources(sender);
    }

    private void RecreateResources(ICanvasResourceCreator resourceCreator)
    {
        // TODO
    }

    private void RenderGraph(CanvasDrawingSession ds)
    {
    }
}
