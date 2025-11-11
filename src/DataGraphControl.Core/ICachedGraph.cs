namespace DataGraphControl.Core;

public interface ICachedGraph : IGraph
{
    IReadOnlyDictionary<uint, INode> NodeCache { get; }
    void RebuildNodeCache();
}
