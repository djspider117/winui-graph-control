using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace DataGraphControl.WinUI;

public partial class StatefulBrush : DependencyObject
{
    [GeneratedDependencyProperty]
    public partial Brush? NormalBrush { get; set; }

    [GeneratedDependencyProperty]
    public partial Brush? MouseOverBrush { get; set; }

    [GeneratedDependencyProperty]
    public partial Brush? PressedBrush { get; set; }

    internal IBrushWithState ToProxy()
    {
        return new BrushWithStateRenderInfo
        {
            MouseOverBrush = MouseOverBrush?.ToBrushProxy(),
            PressedBrush = PressedBrush?.ToBrushProxy(),
            NormalBrush = NormalBrush?.ToBrushProxy()
        };
    }
}
