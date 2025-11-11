using GhostCore.Data.Evaluation;
using System.Collections.Frozen;

namespace DataGraphControl.Core;

public class DataGraph : ICachedGraph, IEvaluatableGraphConnector
{
    private List<Connection> _connections = [];
    private List<INode> _nodes = [];
    private FrozenDictionary<uint, INode>? _nodeCache;

    public IReadOnlyCollection<Connection> Connections => _connections;
    public IReadOnlyCollection<INode> Nodes => _nodes;
    public IReadOnlyDictionary<uint, INode> NodeCache => _nodeCache ?? throw new InvalidOperationException("Please rebuild the node cache.");

    public void RebuildNodeCache()
    {
        _nodeCache = Nodes.ToFrozenDictionary(x => x.Id);
        // TODO, DFS over the nodes and put update Connections
    }

    public void Connect(EvaluatableNode source, string sourcePortName, EvaluatableNode dest, string destPortName, IConverter? converter = null)
    {
        var pc = dest.Data.ConnectToPort(destPortName, source.Data, sourcePortName, converter);
        _connections.Add(new Connection(
            source.Id,
            pc.SourcePort.Id,
            dest.Id,
            dest.Data.GetInputPort(destPortName).Id));
    }

    // default source to output
    public void Connect(EvaluatableNode source, EvaluatableNode dest, string destPortName, IConverter? converter = null)
    {
        var pc = dest.Data.ConnectToPort(destPortName, source.Data, "Output", converter);
        _connections.Add(new Connection(
            source.Id,
            pc.SourcePort.Id,
            dest.Id,
            dest.Data.GetInputPort(destPortName).Id));
    }

    public void AddNode(INode node) => _nodes.Add(node);
    public void AddNodes(params IEnumerable<INode> nodes) => _nodes.AddRange(nodes);
    public void AddConnection(Connection conn) => _connections.Add(conn);

}