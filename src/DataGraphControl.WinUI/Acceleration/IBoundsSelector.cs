namespace DataGraphControl.WinUI.Acceleration;

public interface IBoundsSelector<T>
{
    Quad GetBounds(T data);
}
