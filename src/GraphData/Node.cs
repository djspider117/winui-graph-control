using System.Numerics;

namespace GraphData;

public class NodeTypeDefinition
{
    public uint TypeId { get; set; }
    public string? TypeName { get; set; }
    public List<NodeProperty> InputProperties { get; set; } = [];
    public List<NodeProperty> OutputProperties { get; set; } = [];

    public bool IsSingleOutput => OutputProperties.Count == 1;
    public NodeProperty Output => IsSingleOutput ? OutputProperties[0] : throw new InvalidOperationException("Not a single output node.");

    public NodeTypeDefinition(uint id, string? name)
    {
        TypeId = id;
        TypeName = name;
    }
}

public class Node
{
    public uint Id { get; set; }
    public string? Name { get; set; }

    public bool Selected { get; set; }

    public NodeTypeDefinition TypeDefinition { get; set; }

    public Vector2 Position { get; set; }

    public object? DataObject { get; set; }

    public Node(NodeTypeDefinition typeDef, uint id, string? name, Vector2? position = null)
    {
        TypeDefinition = typeDef;
        Id = id;
        Name = name;
        Position = position ?? Vector2.Zero;
    }
}

//public interface IEvaluatable
//{
//    object GetOutputValue(NodeProperty prop);
//    IReadOnlyCollection<NodeProperty> GetOutputs();
//}

//public abstract class EvaluatableBase : IEvaluatable
//{
//    protected readonly Node _typeDef;

//    public EvaluatableBase(Node nodeTypeDefinition)
//    {
//        _typeDef = nodeTypeDefinition;
//    }

//    public abstract object GetOutputValue(NodeProperty prop);
//    public IReadOnlyCollection<NodeProperty> GetOutputs() => _typeDef.OutputProperties;
//}

//public class V3Data : EvaluatableBase
//{
//    public float X { get; set; }
//    public float y { get; set; }
//    public float Z { get; set; }

//    public Vector3 Output => new(X, y, Z);

//    public V3Data() : base(NodeBuilder.Create("Vector3")
//        .WithInputProperty(MetadataCache.GetPropertyByName("X"))
//        .WithInputProperty(MetadataCache.GetPropertyByName("Y"))
//        .WithInputProperty(MetadataCache.GetPropertyByName("Z"))
//        .WithOutputProperty(MetadataCache.GetPropertyByName("Output"))
//        .Build())
//    {

//    }

//    public virtual object GetOutputValue(NodeProperty props)
//    {
        
//    }
//}

//public class AddV3 : IEvaluatable
//{
//    public IEvaluatable? A { get; set; }
//    public IEvaluatable? B { get; set; }

//    public object GetOutputValue(NodeProperty prop)
//    {
//        Vector3 sum = default;

//        sum += A?.GetOutput()

//        return sum;
//    }
//}