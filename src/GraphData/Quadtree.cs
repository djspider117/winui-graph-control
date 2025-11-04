using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphData;

public class NodeBoundsSelector : IBoundsSelector<Node>
{
    public Vector4 GetBounds(Node value)
    {
        // hardcoded width and height for now
        return new Vector4(value.Position.X, value.Position.Y, value.Position.X + 150, value.Position.Y + 150);
    }
}

public class QuadTree<T>
{
    public int MaxObjects { get; }
    public int MaxLevel { get; }
    public int ObjectCount { get; private set; }
    public Vector4 MainRect { get; }

    private Sector<T> _rootSector;
    private readonly IBoundsSelector<T> _objectBounds;

    public QuadTree(float x, float y, float width, float height, IBoundsSelector<T> objectBounds, int maxObjects = 100, int maxLevel = 5)
    {
        if (objectBounds == null)
            throw new ArgumentNullException(nameof(objectBounds));

        MaxObjects = maxObjects;
        MaxLevel = maxLevel;
        MainRect = new Vector4(x, y, width, height);
        _objectBounds = objectBounds;
        _rootSector = new LeafSector<T>(0, MainRect, objectBounds, maxObjects, maxLevel);
    }

    /// <summary> Initializes a new instance of the <see cref="T:UltimateQuadTree.QuadTree`1"></see> class. </summary>
    /// <param name="width">The width of the boundary rectangle.</param>
    /// <param name="height">The height of the boundary rectangle.</param>
    /// <param name="objectBounds">The set of methods for getting boundaries of an element.</param>
    /// <param name="maxObjects">The max number of elements in one rectangle. The default is 10.</param>
    /// <param name="maxLevel">The max depth level. The default is 5. </param>
    public QuadTree(float width, float height, IBoundsSelector<T> objectBounds, int maxObjects = 100, int maxLevel = 5)
        : this(0, 0, width, height, objectBounds, maxObjects, maxLevel)
    {
    }

    /// <summary> Removes all elements from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
    public void Clear()
    {
        _rootSector.Clear();
        _rootSector = new LeafSector<T>(0, MainRect, _objectBounds, MaxObjects, MaxLevel);
        ObjectCount = 0;
    }

    /// <summary> Inserts an element into the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
    /// <param name="obj">The object to insert.</param>
    /// <returns>true if the element is added to the <see cref="T:UltimateQuadTree.QuadTree`1"></see>; false if failure.</returns>
    public bool Insert(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (!IsObjectInMainRect(obj))
            return false;

        if (_rootSector.TryInsert(obj))
        {
            ObjectCount++;
            return true;
        }

        _rootSector = _rootSector.Quarter();
        _rootSector.TryInsert(obj);

        ObjectCount++;

        return true;
    }

    private bool IsObjectInMainRect(T obj)
    {
        if (_objectBounds.GetBounds(obj).GetTop() > MainRect.GetBottom())
            return false;
        if (_objectBounds.GetBounds(obj).GetBottom() < MainRect.GetTop())
            return false;
        if (_objectBounds.GetBounds(obj).GetLeft() > MainRect.GetRight())
            return false;
        if (_objectBounds.GetBounds(obj).GetRight() < MainRect.GetLeft())
            return false;

        return true;
    }

    /// <summary> Inserts the elements of a collection into the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
    /// <param name="objects">The objects to insert.</param>
    public void InsertRange(IEnumerable<T> objects)
    {
        if (objects == null)
            throw new ArgumentNullException(nameof(objects));

        foreach (var obj in objects)
            Insert(obj);
    }

    /// <summary> Removes the specified element from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
    /// <param name="obj">The object to remove.</param>
    /// <returns>true if the element is successfully found and removed; false if failure.</returns>
    public bool Remove(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (!_rootSector.Remove(obj))
            return false;

        ObjectCount--;

        if (ObjectCount >= MaxObjects)
            return true;

        if (_rootSector.TryCollapse(out var collapsed))
            _rootSector = collapsed;

        return true;
    }

    /// <summary> Removes a range of elements from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
    /// <param name="objects">The objects to remove.</param> 
    public void RemoveRange(IEnumerable<T> objects)
    {
        if (objects == null)
            throw new ArgumentNullException(nameof(objects));

        foreach (var obj in objects)
            Remove(obj);
    }

    /// <summary> Returns the elements nearest to the object. </summary>
    /// <param name="obj">The object for search of the nearest elements.</param> 
    /// <returns> the enumeration of elements nearest to the object. </returns>
    public IEnumerable<T> GetNearestObjects(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        return _rootSector.GetNearestObjects(obj).Distinct();
    }

    /// <summary> Returns all elements from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
    /// <returns> the enumeration of all elements of the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </returns>
    public IEnumerable<T> GetObjects()
    {
        return _rootSector.GetObjects().Distinct();
    }

    /// <summary> Returns all rectangles from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
    /// <returns> the enumeration of all rectangles of the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </returns>
    public IEnumerable<Vector4> GetGrid() => _rootSector.GetRects();
}

public interface IBoundsSelector<in T>
{
    Vector4 GetBounds(T value);
}

internal static class Vector4RectExtensions
{
    internal static float GetTop(this Vector4 v) => v.Y;
    internal static float GetBottom(this Vector4 v) => v.Y + v.W;
    internal static float GetLeft(this Vector4 v) => v.X;
    internal static float GetRight(this Vector4 v) => v.X + v.Z;

    internal static float GetMidX(this Vector4 v) => v.X + v.GetHalfWidth();
    internal static float GetMidY(this Vector4 v) => v.Y + v.GetHalfHeight();

    internal static float GetHalfWidth(this Vector4 v) => v.Z * 0.5f;
    internal static float GetHalfHeight(this Vector4 v) => v.W * 0.5f;

    internal static Vector4 GetLeftTopQuarter(this Vector4 v) => new(v.X, v.Y, v.GetHalfWidth(), v.GetHalfHeight());
    internal static Vector4 GetLeftBottomQuarter(this Vector4 v) => new(v.X, v.GetMidY(), v.GetHalfWidth(), v.GetHalfHeight());
    internal static Vector4 GetRightTopQuarter(this Vector4 v) => new(v.GetMidX(), v.Y, v.GetHalfWidth(), v.GetHalfHeight());
    internal static Vector4 GetRightBottomQuarter(this Vector4 v) => new(v.GetMidX(), v.GetMidY(), v.GetHalfWidth(), v.GetHalfHeight());
}

internal abstract class Sector<T>
{
    protected readonly int MaxObjects;
    protected readonly int MaxLevel;

    protected readonly int Level;
    protected readonly Vector4 Rect;

    protected readonly IBoundsSelector<T> ObjectBounds;

    protected Sector(int level, Vector4 rect, IBoundsSelector<T> objectBounds, int maxObjects, int maxLevel)
    {
        Level = level;
        Rect = rect;
        ObjectBounds = objectBounds;
        MaxObjects = maxObjects;
        MaxLevel = maxLevel;
    }

    public abstract void Clear();
    public abstract bool TryInsert(T obj);
    public abstract Sector<T> Quarter();
    public abstract bool Remove(T obj);
    public abstract bool TryCollapse(out Sector<T> sector);
    public abstract IEnumerable<T> GetNearestObjects(T obj);
    public abstract IEnumerable<T> GetObjects();
    public abstract IEnumerable<Vector4> GetRects();
}

internal class LeafSector<T> : Sector<T>
{
    private readonly HashSet<T> _objects = [];

    public LeafSector(int level, Vector4 rect, IBoundsSelector<T> objectBounds, int maxObjects, int maxLevel)
        : base(level, rect, objectBounds, maxObjects, maxLevel)
    {
    }

    public override void Clear() => _objects.Clear();

    public override bool TryInsert(T obj)
    {
        if (_objects.Count >= MaxObjects && Level < MaxLevel)
            return false;
        _objects.Add(obj);
        return true;
    }

    public override Sector<T> Quarter()
    {
        var node = new NodeSector<T>(Level, Rect, ObjectBounds, MaxObjects, MaxLevel);
        foreach (var o in _objects)
            node.TryInsert(o);
        return node;
    }

    public override bool Remove(T obj)
    {
        return _objects.Remove(obj);
    }

    public override bool TryCollapse(out Sector<T> sector)
    {
        sector = this;
        return false;
    }

    public override IEnumerable<T> GetNearestObjects(T obj)
    {
        return _objects;
    }

    public override IEnumerable<T> GetObjects()
    {
        return _objects;
    }

    public override IEnumerable<Vector4> GetRects()
    {
        yield return Rect;
    }
}

internal class NodeSector<T> : Sector<T>
{
    private int _objectsCount;

    private Sector<T>? _leftTopSector;
    private Sector<T>? _leftBottomSector;
    private Sector<T>? _rightTopSector;
    private Sector<T>? _rightBottomSector;

    public NodeSector(int level, Vector4 rect, IBoundsSelector<T> objectBounds, int maxObjects, int maxLevel)
        : base(level, rect, objectBounds, maxObjects, maxLevel)
    {
        CreateLeaves();
    }

    public override void Clear()
    {
        _leftTopSector?.Clear();
        _leftTopSector = null;

        _leftBottomSector?.Clear();
        _leftBottomSector = null;

        _rightTopSector?.Clear();
        _rightTopSector = null;

        _rightBottomSector?.Clear();
        _rightBottomSector = null;

        _objectsCount = 0;
    }

    public override bool TryInsert(T obj)
    {
        var result = false;

        if (IsLeft(obj))
        {
            if (IsTop(obj))
                result |= Insert(ref _leftTopSector!, obj);
            if (IsBottom(obj))
                result |= Insert(ref _leftBottomSector!, obj);
        }

        if (IsRight(obj))
        {
            if (IsTop(obj))
                result |= Insert(ref _rightTopSector!, obj);
            if (IsBottom(obj))
                result |= Insert(ref _rightBottomSector!, obj);
        }

        if (result)
            _objectsCount++;

        return result;
    }

    private static bool Insert(ref Sector<T> sector, T obj)
    {
        if (sector.TryInsert(obj))
            return true;
        sector = sector.Quarter();
        return sector.TryInsert(obj);
    }

    public override Sector<T> Quarter() => this;

    public override bool Remove(T obj)
    {
        var result = false;

        if (IsLeft(obj))
        {
            if (IsTop(obj))
                result |= Remove(ref _leftTopSector!, obj);
            if (IsBottom(obj))
                result |= Remove(ref _leftBottomSector!, obj);
        }

        if (IsRight(obj))
        {
            if (IsTop(obj))
                result |= Remove(ref _rightTopSector!, obj);
            if (IsBottom(obj))
                result |= Remove(ref _rightBottomSector!, obj);
        }

        if (result)
            _objectsCount--;

        return result;
    }

    private static bool Remove(ref Sector<T> sector, T obj)
    {
        var result = sector.Remove(obj);
        if (!result)
            return false;

        if (sector.TryCollapse(out Sector<T> collapsed))
            sector = collapsed;

        return true;
    }

    public override bool TryCollapse(out Sector<T> sector)
    {
        if (_objectsCount >= MaxObjects)
        {
            sector = this;
            return false;
        }

        sector = new LeafSector<T>(Level, Rect, ObjectBounds, MaxObjects, MaxLevel);
        foreach (var o in GetObjects().Distinct())
            sector.TryInsert(o);

        Clear();

        return true;
    }

    public override IEnumerable<T> GetNearestObjects(T obj)
    {
        return GetSector(obj).SelectMany(s => s.GetNearestObjects(obj));
    }

    public override IEnumerable<T> GetObjects()
    {
        foreach (var obj in _leftTopSector?.GetObjects() ?? [])
            yield return obj;
        foreach (var obj in _leftBottomSector?.GetObjects() ?? [])
            yield return obj;
        foreach (var obj in _rightTopSector?.GetObjects() ?? [])
            yield return obj;
        foreach (var obj in _rightBottomSector?.GetObjects() ?? [])
            yield return obj;
    }

    public override IEnumerable<Vector4> GetRects()
    {
        yield return Rect;

        foreach (var rect in _leftTopSector?.GetRects() ?? [])
            yield return rect;
        foreach (var rect in _leftBottomSector?.GetRects() ?? [])
            yield return rect;
        foreach (var rect in _rightTopSector?.GetRects() ?? [])
            yield return rect;
        foreach (var rect in _rightBottomSector?.GetRects() ?? [])
            yield return rect;
    }

    private void CreateLeaves()
    {
        var nextLevel = Level + 1;

        _leftTopSector = new LeafSector<T>(nextLevel, Rect.GetLeftTopQuarter(), ObjectBounds, MaxObjects, MaxLevel);
        _leftBottomSector = new LeafSector<T>(nextLevel, Rect.GetLeftBottomQuarter(), ObjectBounds, MaxObjects, MaxLevel);
        _rightTopSector = new LeafSector<T>(nextLevel, Rect.GetRightTopQuarter(), ObjectBounds, MaxObjects, MaxLevel);
        _rightBottomSector = new LeafSector<T>(nextLevel, Rect.GetRightBottomQuarter(), ObjectBounds, MaxObjects, MaxLevel);
    }

    private IEnumerable<Sector<T>> GetSector(T obj)
    {
        if (IsLeft(obj))
        {
            if (IsTop(obj))
                yield return _leftTopSector!;
            if (IsBottom(obj))
                yield return _leftBottomSector!;
        }

        if (IsRight(obj))
        {
            if (IsTop(obj))
                yield return _rightTopSector!;
            if (IsBottom(obj))
                yield return _rightBottomSector!;
        }
    }

    private bool IsLeft(T obj) => ObjectBounds.GetBounds(obj).X <= Rect.GetMidX();
    private bool IsTop(T obj) => ObjectBounds.GetBounds(obj).Y <= Rect.GetMidY();
    private bool IsRight(T obj) => ObjectBounds.GetBounds(obj).Z >= Rect.GetMidX();
    private bool IsBottom(T obj) => ObjectBounds.GetBounds(obj).W >= Rect.GetMidY();
}