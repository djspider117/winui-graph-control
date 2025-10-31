using System.Collections.Frozen;
using System.Numerics;

namespace GraphData;

public static class DemoData
{
    public static Graph CreateDemoGraph()
    {
        var rv = new Graph();

        var rectNode = NodeBuilder.Create("Rect")
            .WithPosition(50, 50)
            .WithInputProperties(KnownProperty.Transform, KnownProperty.Color, KnownProperty.Size)
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var textNode = NodeBuilder.Create("SimpleText")
            .WithPosition(50, 260)
            .WithInputProperties(KnownProperty.Transform, KnownProperty.Color, KnownProperty.Text)
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var merge = NodeBuilder.Create("Merge")
            .WithPosition(240, 150)
            .WithInputProperty(KnownProperty.Texture)
            .WithInputProperty(MetadataCache.GetPropertyByName("Texture2"))
            .WithInputProperty(MetadataCache.GetPropertyByName("Mask"))
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var renderer = NodeBuilder.Create("Renderer")
            .WithPosition(420, 150)
            .WithInputProperty(KnownProperty.Texture)
            .Build();

        rv.Nodes.AddRange(rectNode, textNode, merge, renderer);
        rv.Connections.Add(new Connection(rectNode.Id, (uint)KnownProperty.Texture, merge.Id, (uint)KnownProperty.Texture));
        rv.Connections.Add(new Connection(textNode.Id, (uint)KnownProperty.Texture, merge.Id, MetadataCache.GetPropertyByName("Texture2").PropertyId));
        rv.Connections.Add(new Connection(merge.Id, (uint)KnownProperty.Texture, renderer.Id, (uint)KnownProperty.Texture));

        rv.RebuildNodeCache();

        return rv;
    }
}

public static class MetadataCache
{
    private static readonly FrozenDictionary<uint, NodeProperty> _propertyCache;

    static MetadataCache()
    {
        _propertyCache = InitProperties();
    }

    private static FrozenDictionary<uint, NodeProperty> InitProperties()
    {
        var dic = new Dictionary<uint, NodeProperty>
        {
            { 0, new NodeProperty(0, nameof(KnownProperty.Transform), (uint)KnownPropertyType.Matrix) },
            { 1, new NodeProperty(1, nameof(KnownProperty.Color), (uint)KnownPropertyType.Vector4) },
            { 3, new NodeProperty(3, nameof(KnownProperty.Text), (uint)KnownPropertyType.String) },
            { 4, new NodeProperty(4, nameof(KnownProperty.Size), (uint)KnownPropertyType.Vector2) },
            { 5, new NodeProperty(5, nameof(KnownProperty.Texture), (uint)KnownPropertyType.Texture2D) },
            { 6, new NodeProperty(6, $"{nameof(KnownProperty.Texture)}2", (uint)KnownPropertyType.Texture2D) },
            { 7, new NodeProperty(7, "Mask", (uint)KnownPropertyType.Texture2D) },
        };

        return dic.ToFrozenDictionary();
    }

    public static NodeProperty GetKnownProperty(KnownProperty prop) => _propertyCache[(uint)prop];
    public static NodeProperty GetPropertyByName(string name) => _propertyCache.Values.First(x => x.Name == name);
}

public enum KnownProperty : uint
{
    Transform = 0,
    Size = 4,
    Texture = 5,
    Color = 1,
    Text = 3
}

public enum KnownPropertyType : uint
{
    Vector2,
    Vector3,
    Vector4,
    Quaternion = Vector4,
    Matrix,
    String,
    Texture2D
}

public readonly record struct Connection
{
    public readonly ulong NodeMask;
    public readonly ulong PropertyMask;

    public uint SourceNodeId => (uint)(NodeMask >> 32);
    public uint TargetNodeId => (uint)(NodeMask & uint.MaxValue);

    public uint SourcePropertyId => (uint)(PropertyMask >> 32);
    public uint TargetPropertyId => (uint)(PropertyMask & uint.MaxValue);

    public Connection(uint sourceNodeId, uint sourcePropId, uint targetNodeId, uint targetPropId)
    {
        NodeMask = (ulong)sourceNodeId << 32 | targetNodeId;
        PropertyMask = (ulong)sourcePropId << 32 | targetPropId;
    }
}

public readonly record struct NodeProperty(uint PropertyId, string Name, uint TypeId);

public class Node
{
    public uint Id { get; set; }
    public string? Name { get; set; }
    public List<NodeProperty> InputProperties { get; set; } = [];
    public List<NodeProperty> OutputProperties { get; set; } = [];

    public Vector2 Position { get; set; }

    public Node(uint id, string? name, Vector2? position = null)
    {
        Id = id;
        Name = name;
        Position = position ?? Vector2.Zero;
    }
}

public class Graph
{
    private FrozenDictionary<uint, Node>? _nodeCache;

    public FrozenDictionary<uint, Node> NodeCache => _nodeCache ?? throw new InvalidOperationException("Please rebuild the node cache.");

    public List<Node> Nodes { get; set; } = [];
    public List<Connection> Connections { get; set; } = [];

    public void RebuildNodeCache() => _nodeCache = Nodes.ToFrozenDictionary(x => x.Id);
}

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