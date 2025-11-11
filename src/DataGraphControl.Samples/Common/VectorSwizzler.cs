using GhostCore.Data.Evaluation.SourceGen;
using System.Numerics;

namespace DataGraphControl.Samples.Common;

[Evaluatable]
public partial class VectorSwizzler
{
    [Input]
    public Vector3 Input { get; set; }

    [Output]
    public Vector3 ZYX => new(Input.Z, Input.Y, Input.X);

    [Output]
    public Vector3 XYY => new(Input.X, Input.Y, Input.Y);

    [Output]
    public Vector3 XXX => new(Input.X, Input.X, Input.X);
}