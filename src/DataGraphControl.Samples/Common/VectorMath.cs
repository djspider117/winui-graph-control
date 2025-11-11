using GhostCore.Data.Evaluation.SourceGen;
using System.Numerics;

namespace DataGraphControl.Samples.Common;

[Evaluatable]
public partial class VectorMath
{
    [Input]
    public Vector3 A { get; set; }

    [Input]
    public Vector3 B { get; set; }

    [Output("A+B")]
    public Vector3 Sum => A + B;

    [Output("A-B")]
    public Vector3 Diff => A - B;

    [Output("A*B")]
    public Vector3 Mul => A * B;

    [Output("Dot Product")]
    public float Dot => Vector3.Dot(A, B);
}
