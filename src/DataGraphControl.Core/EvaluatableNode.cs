using GhostCore.Data.Evaluation;
using System.Numerics;

namespace DataGraphControl.Core;

public class EvaluatableNode : Node<IEvaluatable>
{
    public EvaluatableNode(IEvaluatable dataObj, uint id, string? name, Vector2? position = null) 
        : base(dataObj, id, name, position)
    {
        Name = name ?? dataObj.TypeDefinition.TypeName;
    }
}