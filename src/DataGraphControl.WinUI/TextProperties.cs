using CommunityToolkit.WinUI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Windows.UI.Text;

namespace DataGraphControl.WinUI;

public partial class TextProperties : DependencyObject
{
    public static readonly TextProperties Common = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 12
    };

    [GeneratedDependencyProperty(DefaultValue = "")]
    public partial string FontFamily { get; set; }

    [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetFontWeightDefault))]
    public partial FontWeight FontWeight { get; set; }
    internal static FontWeight GetFontWeightDefault => FontWeights.Normal;

    [GeneratedDependencyProperty]
    public partial FontStyle FontStyle { get; set; }

    [GeneratedDependencyProperty]
    public partial int FontSize { get; set; }

    internal TextPropertiesRenderInfo GetRenderInfo()
    {
        return new()
        {
            FontFamily = FontFamily,
            FontWeight = FontWeight.Weight,
            FontSize = FontSize,
            Italic = (int)FontStyle
        };
    }
}
