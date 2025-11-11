using GhostCore.Data.Evaluation;
using System.Numerics;

namespace DataGraphControl.Core;

public interface INode
{
    uint Id { get; }
    string? Name { get; set; }
    Vector2 Position { get; set; }
    DynamicTypeDefinition TypeDefinition { get; }
}
