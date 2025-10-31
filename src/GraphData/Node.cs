using System.Numerics;

namespace GraphData;

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
