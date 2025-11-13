using DataGraphControl.Core;
using DataGraphControl.WinUI.Rendering.Context;
using System.Numerics;
using Windows.UI;

namespace DataGraphControl.WinUI.Rendering;

public partial class ConnectionRenderContext(Connection Data,
    float Thickness,
    Color Color,
    Vector2 Start,
    Vector2 End,
    Vector2 Control1,
    Vector2 Control2) : ElementRenderContext
{
    public Connection Data { get; } = Data;
    public float Thickness { get; set; } = Thickness;
    public Color Color { get; set; } = Color;
    public Vector2 Start { get; set; } = Start;
    public Vector2 End { get; set; } = End;
    public Vector2 Control1 { get; set; } = Control1;
    public Vector2 Control2 { get; set; } = Control2;
}
