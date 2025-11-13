using CommunityToolkit.WinUI;
using DataGraphControl.Core;
using DataGraphControl.WinUI.Rendering;
using DataGraphControl.WinUI.Rendering.Default;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DataGraphControl.WinUI;
public sealed partial class DataGraphView : UserControl
{
    private NodeStyleRenderInfo? _nodeStyleRenderInfo;
    private GraphRenderer? _renderer;
    private volatile bool _invaldated = false;

    [GeneratedDependencyProperty]
    public partial IGraph? GraphData { get; set; }
    partial void OnGraphDataChanged(IGraph? newValue)
    {
        if (newValue == null)
            return;

        _renderer?.ResetGraph(newValue);
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
        Loaded += DataGraphView_Loaded;
        Unloaded += DataGraphView_Unloaded;
    }

    private void DataGraphView_Loaded(object sender, RoutedEventArgs e)
    {
        _renderer = new(GraphData, new());
    }

    private void DataGraphView_Unloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= DataGraphView_Loaded;
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

        _renderer?.Render(sender, args);
    }

    private void NodeCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        RecreateResources(sender);
    }

    private void RecreateResources(ICanvasResourceCreator resourceCreator)
    {
        if (_renderer == null)
            _invaldated = true;
        else
            _renderer.Invalidate(resourceCreator);
    }
}
