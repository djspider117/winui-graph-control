using GhostCore.Data.Evaluation;
using System;
using System.Numerics;

namespace DataGraphControl.Samples.Common;

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
