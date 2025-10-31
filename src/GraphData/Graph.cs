using System.Collections.Frozen;

namespace GraphData;

public class Graph
{
    private FrozenDictionary<uint, Node>? _nodeCache;

    public FrozenDictionary<uint, Node> NodeCache => _nodeCache ?? throw new InvalidOperationException("Please rebuild the node cache.");

    public List<Node> Nodes { get; set; } = [];
    public List<Connection> Connections { get; set; } = [];

    public void RebuildNodeCache() => _nodeCache = Nodes.ToFrozenDictionary(x => x.Id);
}
