using System.Numerics;
using Windows.UI;

namespace DataGraphControl.WinUI.Extensions;

public static class VectorExtensions
{
    public static Color ToColor(this Vector4 v) => Color.FromArgb((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W);
}
