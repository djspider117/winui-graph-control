namespace GraphData;

public enum KnownPropertyType : uint
{
    Float,
    Vector2,
    Vector3,
    Vector4,
    Quaternion = Vector4,
    Matrix3x2,
    Matrix4x4,
    String,
    Texture2D
}
