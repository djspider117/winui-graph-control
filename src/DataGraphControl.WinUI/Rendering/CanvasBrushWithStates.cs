using Microsoft.Graphics.Canvas.Brushes;
using System;

namespace DataGraphControl.WinUI.Rendering;

public sealed partial class CanvasBrushWithStates : IDisposable
{
    public ICanvasBrush NormalBrush { get; set; }
    public ICanvasBrush? MouseOverBrush { get; set; }
    public ICanvasBrush? PressedBrush { get; set; }
    public ICanvasBrush? SelectedBrush { get; set; }

    public CanvasBrushWithStates(ICanvasBrush normal, ICanvasBrush? selected = null)
    {
        NormalBrush = normal;
        SelectedBrush = selected;
    }

    public void Dispose()
    {
        NormalBrush?.Dispose();
        MouseOverBrush?.Dispose();
        PressedBrush?.Dispose();
        SelectedBrush?.Dispose();
    }
}