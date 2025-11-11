using GhostCore.Data.Evaluation.SourceGen;
using System.Numerics;

namespace DataGraphControl.Samples.Common;

[Evaluatable]
public partial class V3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    [Output]
    public Vector3 Output => new(X, Y, Z);
}
