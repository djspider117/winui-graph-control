using GhostCore.Data.Evaluation;
using GhostCore.Data.Evaluation.SourceGen;
using System.Numerics;

namespace GraphData;

public static class DemoData
{
    public static Graph CreateDemoGraph()
    {
        var s = new Scalar { X = 10 };

        var v1 = new V3 { X = 0, Y = 1, Z = 2 };
        var v2 = new V3 { X = 2, Y = 1, Z = 0 };
        var v3 = new V3 { X = 3, Y = 2, Z = 1 };

        var math1 = new VectorMath();
        var math2 = new VectorMath();
        var math3 = new VectorMath();
        var swizler = new VectorSwizzler();
        swizler.ConnectToPort("Input", math3, "Mul");

        var rv = new Graph();

        var nv1 = new Node(v1, 1, null, new(180, 77));
        var nv2 = new Node(v2, 2, null, new(180, 313));
        var nm1 = new Node(math1, 4, null, new(409, 209));
        var nv3 = new Node(v3, 3, null, new(409, 458));
        var nm2 = new Node(math2, 5, null, new(666, 308));
        var ns = new Node(s, 0, null, new(666, 52));
        var nm3 = new Node(math3, 6, null, new(878, 193));
        var nsz = new Node(swizler, 7, null, new(1106, 193));

        rv.Nodes.AddRange(ns, nv1, nv2, nv3, nm1, nm2, nm3, nsz);

        rv.Connect(nv1, nm1, nameof(VectorMath.A));
        rv.Connect(nv2, nm1, nameof(VectorMath.B));

        rv.Connect(nm1, nameof(VectorMath.Sum), nm2, nameof(VectorMath.A));
        rv.Connect(nv3, nm2, nameof(VectorMath.B));

        rv.Connect(nm2, nameof(VectorMath.Sum), nm3, nameof(VectorMath.A));
        rv.Connect(ns, nm3, nameof(VectorMath.B), FloatToVector3Converter.Instance);

        rv.Connect(nm3, "Mul", nsz, "Input");
        rv.RebuildNodeCache();

        return rv;
    }
}


public class FloatToVector3Converter : IConverter
{
    public static readonly FloatToVector3Converter Instance = new();

    public object? Convert(object? data, Type? sourceType, Type targetType)
    {
        if (data is not float f)
            throw new InvalidOperationException("Invalid conversion. source not float");

        return new Vector3(f, f, f);
    }
}

[Evaluatable]
public partial class Scalar
{
    public float X { get; set; }

    [Output]
    public float Output => X;
}

[Evaluatable]
public partial class V3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    [Output]
    public Vector3 Output => new(X, Y, Z);
}

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