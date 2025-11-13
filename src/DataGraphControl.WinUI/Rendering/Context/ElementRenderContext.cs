using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DataGraphControl.WinUI.Rendering.Context;

public abstract class ElementRenderContext : IDisposable
{
    public static readonly ElementRenderContext Empty = null!;

    public List<IDisposable> DisposableResources { get; set; } = [];

    public VisualState VisualState { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public bool Dirty { get; set; } = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void SetDisposableProperty<T>(T? value, ref T? backingField)
        where T : IDisposable
    {
        if (backingField != null)
            DisposableResources.Remove(backingField);

        backingField = value;
        if (backingField != null)
            DisposableResources.Add(backingField);
    }

    public virtual void Dispose()
    {
        foreach (var dsp in DisposableResources)
        {
            dsp?.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}

public record struct VisualState(bool IsEnabled = true, bool IsSelected = false, bool IsMouseOver = false);