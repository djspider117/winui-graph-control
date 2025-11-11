using GhostCore.Data.Evaluation.SourceGen;

namespace DataGraphControl.Samples.Common;

[Evaluatable]
public partial class Scalar
{
    public float X { get; set; }

    [Output]
    public float Output => X;
}
