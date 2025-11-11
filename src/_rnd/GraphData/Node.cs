using GhostCore.Data.Evaluation;
using System.Numerics;

namespace GraphData;

public class Node
{
    public uint Id { get; set; }
    public string? Name { get; set; }
    public Vector2 Position { get; set; }

    public IEvaluatable DataObject { get; set; }
    public bool Selected { get; set; }

    public Node(IEvaluatable dataObj, uint id, string? name, Vector2? position = null)
    {
        DataObject = dataObj;
        Id = id;
        Name = name ?? dataObj.TypeDefinition.TypeName;
        Position = position ?? Vector2.Zero;
    }
}