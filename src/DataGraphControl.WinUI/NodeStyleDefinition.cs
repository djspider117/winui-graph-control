using CommunityToolkit.WinUI;
using DataGraphControl.WinUI.Extensions;
using DataGraphControl.WinUI.Rendering.Default;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Numerics;

namespace DataGraphControl.WinUI;

public partial class NodeStyleDefinition : DependencyObject
{
    public static readonly NodeStyleDefinition Default;

    static NodeStyleDefinition()
    {
        Default = new()
        {
            RowHeight = 27,
            BackgroundBrush = new SolidColorBrush(Colors.Gray),
            BorderBrush = new SolidColorBrush(Colors.LightGray),
            BorderThickness = 2,
            CornerRadiusX = 8,
            CornerRadiusY = 8,
            MinHeight = 150,
            MaxHeight = 300,
            MinWidth = 150,
            MaxWidth = 200,
            Orientation = Orientation.Vertical,
            ShowPorts = true,
            TitleBrush = new SolidColorBrush(Colors.White),
            TitleTextProperties = TextProperties.Common,
            InputPortStyle = new()
            {
                PortBrush = new StatefulBrush
                {
                    NormalBrush = new SolidColorBrush(Colors.LightGray),
                    MouseOverBrush = new SolidColorBrush(Colors.Red),
                    PressedBrush = new SolidColorBrush(Colors.DarkRed),
                },
                PortRadius = 5,
                TextBrush = new SolidColorBrush(Colors.LightGray),
                TextProperties = TextProperties.Common
            }
        };
    }

    internal event PropertyChangedCallback? InternalPropertyChanged;

    [GeneratedDependencyProperty]
    public partial int RowHeight { get; set; }

    [GeneratedDependencyProperty]
    public partial double CornerRadiusX { get; set; }

    [GeneratedDependencyProperty]
    public partial double CornerRadiusY { get; set; }

    [GeneratedDependencyProperty]
    public partial double MinWidth { get; set; }

    [GeneratedDependencyProperty]
    public partial double MaxWidth { get; set; }

    [GeneratedDependencyProperty]
    public partial double MinHeight { get; set; }

    [GeneratedDependencyProperty]
    public partial double MaxHeight { get; set; }

    [GeneratedDependencyProperty]
    public partial double BorderThickness { get; set; }

    [GeneratedDependencyProperty]
    public partial Brush? BorderBrush { get; set; }

    [GeneratedDependencyProperty]
    public partial Brush? BackgroundBrush { get; set; }

    [GeneratedDependencyProperty]
    public partial Brush? TitleBrush { get; set; }

    [GeneratedDependencyProperty]
    public partial TextProperties? TitleTextProperties { get; set; }

    [GeneratedDependencyProperty]
    public partial bool ShowPorts { get; set; }
    
    [GeneratedDependencyProperty]
    public partial Orientation Orientation { get; set; }

    [GeneratedDependencyProperty]
    public partial PortStyleDefinition? InputPortStyle { get; set; }

    [GeneratedDependencyProperty]
    public partial PortStyleDefinition? OutputPortStyle { get; set; }

    internal NodeStyleRenderInfo GetRenderInfo()
    {
        return new NodeStyleRenderInfo
        {
            RowHeight = RowHeight,
            CornerRadius = new Vector2((float)CornerRadiusX, (float)CornerRadiusY),
            MinMaxWidth = new Vector2((float)MinWidth, (float)MaxWidth),
            MinMaxHeight = new Vector2((float)MinHeight, (float)MaxHeight),
            BorderThickness = (float)BorderThickness,
            BorderBrush = BorderBrush?.ToBrushProxy(),
            BackgroundBrush = BackgroundBrush?.ToBrushProxy(),
            TitleBrush = TitleBrush?.ToBrushProxy(),
            TitleTextProperties = TitleTextProperties?.GetRenderInfo(),
            ShowPorts = ShowPorts,
            Orientation = Orientation,
            InputPortStyle = InputPortStyle?.GetRenderInfo(),
            OutputPortStyle = OutputPortStyle?.GetRenderInfo() ?? InputPortStyle?.GetRenderInfo()
        };
    }
}
