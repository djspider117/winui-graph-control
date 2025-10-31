using System.Numerics;

namespace GraphData;

public class NodeBuilder
{
    private static readonly Random _random = new();
    private readonly Node _curNode;
    private bool _built = false;

    private NodeBuilder(uint id, string name)
    {
        _curNode = new Node(id, name);
    }

    public static NodeBuilder Create(uint id, string name)
    {
        return new NodeBuilder(id, name);
    }

    public static NodeBuilder Create(string name)
    {
        return new NodeBuilder(Convert.ToUInt32(_random.Next()), name);
    }

    public NodeBuilder WithInputProperty(NodeProperty prop)
    {
        _curNode?.InputProperties.Add(prop);
        return this;
    }
    public NodeBuilder WithOutputProperty(NodeProperty prop)
    {
        _curNode?.OutputProperties.Add(prop);
        return this;
    }

    public NodeBuilder WithInputProperty(KnownProperty prop)
    {
        _curNode?.InputProperties.Add(MetadataCache.GetKnownProperty(prop));
        return this;
    }
    public NodeBuilder WithOutputProperty(KnownProperty prop)
    {
        _curNode?.OutputProperties.Add(MetadataCache.GetKnownProperty(prop));
        return this;
    }

    public NodeBuilder WithInputProperties(params NodeProperty[] props)
    {
        _curNode?.InputProperties.AddRange(props);
        return this;
    }
    public NodeBuilder WithOutputProperties(params NodeProperty[] props)
    {
        _curNode?.OutputProperties.AddRange(props);
        return this;
    }

    public NodeBuilder WithInputProperties(params KnownProperty[] props)
    {
        foreach (var prop in props)
        {
            _curNode?.InputProperties.Add(MetadataCache.GetKnownProperty(prop));
        }
        return this;
    }
    public NodeBuilder WithOutputProperties(params KnownProperty[] props)
    {
        foreach (var prop in props)
        {
            _curNode?.OutputProperties.Add(MetadataCache.GetKnownProperty(prop));
        }
        return this;
    }

    public NodeBuilder WithPosition(Vector2 position)
    {
        _curNode.Position = position;
        return this;
    }
    public NodeBuilder WithPosition(int x, int y)
    {
        _curNode.Position = new Vector2(x, y);
        return this;
    }

    public Node Build()
    {
        if (_built)
            throw new InvalidOperationException("Node already built");

        _built = true;
        return _curNode;
    }
}