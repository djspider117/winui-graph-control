using GhostCore.Data.Evaluation;

namespace DataGraphControl.Core;

public interface IEvaluatableGraphConnector
{
    void Connect(EvaluatableNode source, EvaluatableNode dest, string destPortName, IConverter? converter = null);
    void Connect(EvaluatableNode source, string sourcePortName, EvaluatableNode dest, string destPortName, IConverter? converter = null);
}
