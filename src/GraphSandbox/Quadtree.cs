using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GraphSandbox;

public interface IBoundsSelector<T>
{
    Quad GetBounds(T data);
}

public readonly struct Quad
{
    public readonly Point Center;
    public readonly double HalfWidth;
    public readonly double HalfHeight;

    public readonly Point TopLeft; 
    public readonly Point TopRight;
    public readonly Point BottomLeft;
    public readonly Point BottomRight; 

    public Quad(Point center, double halfSize)
    {
        Center = center;
        HalfWidth = halfSize;
        HalfHeight = halfSize;
        TopLeft = new(center.X - halfSize, center.Y + halfSize);
        TopRight = new(center.X + halfSize, center.Y + halfSize);
        BottomLeft = new(center.X - halfSize, center.Y - halfSize);
        BottomRight = new(center.X + halfSize, center.Y - halfSize);

    }
    public Quad(double x, double y, double width, double height)
    {
        TopLeft = new Point(x, y);
        TopRight = new Point(x + width, y);
        BottomLeft = new Point(x, y + height);
        BottomRight = new Point(x + width, y + height);

        HalfWidth = width * 0.5;
        HalfHeight = height * 0.5;
        Center = new Point(x + HalfWidth, y + HalfHeight);
    }
    public Quad(Point pt, double width, double height) : this(pt.X, pt.Y, width, height) { }
    public Quad(Point tl, Point tr, Point bl, Point br)
    {
        TopLeft = tl;
        TopRight = tr;
        BottomLeft = bl;
        BottomRight = br;

        var width = tr.X - tl.X;
        var height = tr.Y - br.Y;
        HalfWidth = width * 0.5;
        HalfHeight = height * 0.5;
        Center = new Point(tl.X + HalfWidth, tl.Y + HalfHeight);
    }

    public bool ContainsPoint(Point pt)
    {
        return TopLeft.X <= pt.X && pt.X <= TopRight.X &&
               TopLeft.Y <= pt.Y && pt.Y <= BottomRight.Y;
    }

    public bool IntersectsQuad(Quad other)
    {
        var centers = new Vector4((float)Center.X, (float)Center.Y, (float)other.Center.X, (float)other.Center.Y);
        var hs = new Vector4((float)HalfWidth, (float)HalfHeight, (float)other.HalfWidth, (float)other.HalfHeight);

        var plus = centers + hs;
        var minus = centers - hs;

        return minus.X <= plus.Z && plus.X >= minus.Z &&
               minus.Y <= plus.W && plus.Y >= minus.W;
    }

    public bool ContainsQuad(Quad other)
    {
        return ContainsPoint(other.TopLeft) &&
               ContainsPoint(other.TopRight) &&
               ContainsPoint(other.BottomLeft) &&
               ContainsPoint(other.BottomRight);
    }

    public (Quad TopLeft, Quad TopRight, Quad BottomLeft, Quad BottomRight) GetSubdivisions()
    {
        var rx = TopLeft.X;
        var ry = TopLeft.Y;

        var tl = new Quad(rx, ry, HalfWidth, HalfHeight);
        var tr = new Quad(rx + HalfWidth, ry, HalfWidth, HalfHeight);
        var bl = new Quad(rx, ry + HalfHeight, HalfWidth, HalfHeight);
        var br = new Quad(rx + HalfWidth, ry + HalfHeight, HalfWidth, HalfHeight);

        return (tl, tr, bl, br);
    }

    public bool CanFitInSubdivisions(Quad target)
    {
        (var nw, var ne, var sw, var se) = target.GetSubdivisions();

        if (nw.ContainsQuad(this))
            return true;

        if (ne.ContainsQuad(this))
            return true;

        if (sw.ContainsQuad(this))
            return true;

        if (se.ContainsQuad(this))
            return true;

        return false;
    }
}

public class Cluster<TData, TBoundsSelector> where TBoundsSelector : IBoundsSelector<TData>, new()
{
    public const int CAPACITY = 10; // TODO set this based on struct size

    private readonly static TBoundsSelector _boundsSelector = new();

    private Cluster<TData, TBoundsSelector>? _northWest;
    private Cluster<TData, TBoundsSelector>? _northEast;
    private Cluster<TData, TBoundsSelector>? _southWest;
    private Cluster<TData, TBoundsSelector>? _southEast;
    private bool _isSubdivided;

    public readonly int Id;
    public readonly Quad Bounds;

    public readonly List<TData> Data;

    public Cluster(Quad bounds, int id)
    {
        Bounds = bounds;
        Data = new(CAPACITY);
        Id = id;
    }

    public bool Insert(TData data)
    {
        var dataBounds = _boundsSelector.GetBounds(data);

        if (!Bounds.ContainsQuad(dataBounds))
            return false; // do we throw exception that the inserted bounds exceedes the cluster bounds?

        if (Data.Count < CAPACITY && !_isSubdivided || !dataBounds.CanFitInSubdivisions(Bounds))
        {
            Data.Add(data);
            return true;
        }

        if (!_isSubdivided)
            Subdivide();

        return InsertInternal(data, dataBounds);
    }
    public IReadOnlyCollection<TData> Query(Quad range)
    {
        var rv = new List<TData>();

        if (!Bounds.IntersectsQuad(range))
            return rv;

        foreach (var data in Data)
        {
            var bounds = _boundsSelector.GetBounds(data);
            if (range.IntersectsQuad(bounds))
                rv.Add(data);
        }

        if (!_isSubdivided)
            return rv;

        if (_northWest!.Bounds.IntersectsQuad(range))
            rv.AddRange(_northWest.Query(range));

        if (_northEast!.Bounds.IntersectsQuad(range))
            rv.AddRange(_northEast.Query(range));

        if (_southWest!.Bounds.IntersectsQuad(range))
            rv.AddRange(_southWest.Query(range));

        if (_southEast!.Bounds.IntersectsQuad(range))
            rv.AddRange(_southEast.Query(range));

        return rv;
    }
    public IReadOnlyCollection<TData> Query(Point point)
    {
        var rv = new List<TData>();

        if (!Bounds.ContainsPoint(point))
            return rv;

        foreach (var data in Data)
        {
            var bounds = _boundsSelector.GetBounds(data);
            if (bounds.ContainsPoint(point))
                rv.Add(data);
        }

        if (!_isSubdivided)
            return rv;

        if (_northWest!.Bounds.ContainsPoint(point))
            rv.AddRange(_northWest.Query(point));

        if (_northEast!.Bounds.ContainsPoint(point))
            rv.AddRange(_northEast.Query(point));

        if (_southWest!.Bounds.ContainsPoint(point))
            rv.AddRange(_southWest.Query(point));

        if (_southEast!.Bounds.ContainsPoint(point))
            rv.AddRange(_southEast.Query(point));

        return rv;
    }
    public void GetQuadTreeStructure(List<RenderingQuad> results)
    {
        results.Add(new RenderingQuad(Bounds, Id));

        _northWest?.GetQuadTreeStructure(results);
        _northEast?.GetQuadTreeStructure(results);
        _southWest?.GetQuadTreeStructure(results);
        _southEast?.GetQuadTreeStructure(results);
    }
    public void Clear()
    {
        Data.Clear();

        // do we actually need to clear everything? will the GC do its job?

        _northWest?.Clear();
        _northWest = null;

        _northEast?.Clear();
        _northEast = null;

        _southWest?.Clear();
        _southWest = null;

        _southEast?.Clear();
        _southEast = null;

    }


    private bool InsertInternal(TData data, Quad bounds)
    {
        if (_northWest?.Bounds.ContainsQuad(bounds) ?? false)
            return _northWest.Insert(data);

        if (_northEast?.Bounds.ContainsQuad(bounds) ?? false)
            return _northEast.Insert(data);

        if (_northWest?.Bounds.ContainsQuad(bounds) ?? false)
            return _northWest.Insert(data);

        if (_southEast?.Bounds.ContainsQuad(bounds) ?? false)
            return _southEast.Insert(data);

        return false;
    }
    private void Subdivide()
    {
        (var nw, var ne, var sw, var se) = Bounds.GetSubdivisions();

        _northWest = new Cluster<TData, TBoundsSelector>(nw, Id / 2);
        _northEast = new Cluster<TData, TBoundsSelector>(ne, Id / 2);
        _southWest = new Cluster<TData, TBoundsSelector>(sw, Id / 2);
        _southEast = new Cluster<TData, TBoundsSelector>(se, Id / 2);

        int i = Data.Count; // prevent infinite loop if InsertInternal fails
        while (i > 0)
        {
            var localData = Data[0];

            if (InsertInternal(localData, _boundsSelector.GetBounds(localData)))
                Data.RemoveAt(0);

            i--;
        }

        _isSubdivided = true;
    }
}

public readonly record struct RenderingQuad(Quad Quad, int Id);