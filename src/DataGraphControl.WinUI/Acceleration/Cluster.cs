using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace DataGraphControl.WinUI.Acceleration;

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
