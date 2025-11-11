using GhostCore.Data.Evaluation;
using System.Numerics;

namespace DataGraphControl.Core;

public class Node<TData> : IDataNode<TData>
{
    public uint Id { get; set; }
    public string? Name { get; set; }
    public Vector2 Position { get; set; }
    public DynamicTypeDefinition TypeDefinition { get; protected set; }

    public TData Data { get; set; }

    public Node(DynamicTypeDefinition typeDefinition, TData dataObj, uint id, string? name, Vector2? position = null)
    {
        TypeDefinition = typeDefinition;
        Data = dataObj;
        Id = id;
        Position = position ?? Vector2.Zero;
        Name = name;
    }

}