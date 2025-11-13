using CommunityToolkit.WinUI;
using DataGraphControl.WinUI.Extensions;
using DataGraphControl.WinUI.Rendering.Default;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace DataGraphControl.WinUI;

public partial class PortStyleDefinition : DependencyObject
{
    [GeneratedDependencyProperty]
    public partial StatefulBrush? PortBrush { get; set; }

    [GeneratedDependencyProperty]
    public partial Brush? TextBrush { get; set; }

    [GeneratedDependencyProperty]
    public partial double PortRadius { get; set; }

    [GeneratedDependencyProperty]
    public partial TextProperties? TextProperties { get; set; }

    internal PortStyleRenderInfo GetRenderInfo()
    {
        return new()
        {
            PortBrush = PortBrush?.ToProxy(),
            PortRadius = (float)PortRadius,
            TextBrush = TextBrush?.ToBrushProxy(),
            TextProperties = TextProperties?.GetRenderInfo(),
        };
    }
}
