namespace DataGraphControl.Core;

public interface IGraph
{
    IReadOnlyCollection<Connection> Connections { get; }
    IReadOnlyCollection<INode> Nodes { get;}
}