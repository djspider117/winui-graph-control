using GhostCore.Data.Evaluation;
using System.Collections.Frozen;
using System.Numerics;

namespace GraphData;

public class Graph
{
    private FrozenDictionary<uint, Node>? _nodeCache;

    public FrozenDictionary<uint, Node> NodeCache => _nodeCache ?? throw new InvalidOperationException("Please rebuild the node cache.");

    public List<Node> Nodes { get; set; } = [];
    public List<Connection> Connections { get; set; } = [];

    public void RebuildNodeCache() => _nodeCache = Nodes.ToFrozenDictionary(x => x.Id);

    internal void CacheConnectionsFromData()
    {
        // TODO, DFS over the nodes and put update Connections
    }

    public void Connect(Node source, string sourcePortName, Node dest, string destPortName, IConverter? converter = null)
    {
        var pc = dest.DataObject.ConnectToPort(destPortName, source.DataObject, sourcePortName, converter);
        Connections.Add(new Connection(
            source.Id,
            pc.SourcePort.Id,
            dest.Id,
            dest.DataObject.GetInputPort(destPortName).Id));
    }

    // default source to output
    public void Connect(Node source, Node dest, string destPortName, IConverter? converter = null)
    {
        var pc = dest.DataObject.ConnectToPort(destPortName, source.DataObject, "Output", converter);
        Connections.Add(new Connection(
            source.Id,
            pc.SourcePort.Id,
            dest.Id,
            dest.DataObject.GetInputPort(destPortName).Id));
    }
}