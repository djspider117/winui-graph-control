using System.Collections.Frozen;

namespace GraphData;

public static class MetadataCache
{
    private static readonly FrozenDictionary<uint, NodeProperty> _propertyCache;

    static MetadataCache()
    {
        _propertyCache = InitProperties();
    }

    private static FrozenDictionary<uint, NodeProperty> InitProperties()
    {
        var dic = new Dictionary<uint, NodeProperty>
        {
            { 0, new NodeProperty(0, nameof(KnownProperty.Transform), (uint)KnownPropertyType.Matrix) },
            { 1, new NodeProperty(1, nameof(KnownProperty.Color), (uint)KnownPropertyType.Vector4) },
            { 3, new NodeProperty(3, nameof(KnownProperty.Text), (uint)KnownPropertyType.String) },
            { 4, new NodeProperty(4, nameof(KnownProperty.Size), (uint)KnownPropertyType.Vector2) },
            { 5, new NodeProperty(5, nameof(KnownProperty.Texture), (uint)KnownPropertyType.Texture2D) },
            { 6, new NodeProperty(6, $"{nameof(KnownProperty.Texture)}2", (uint)KnownPropertyType.Texture2D) },
            { 7, new NodeProperty(7, "Mask", (uint)KnownPropertyType.Texture2D) },
        };

        return dic.ToFrozenDictionary();
    }

    public static NodeProperty GetKnownProperty(KnownProperty prop) => _propertyCache[(uint)prop];
    public static NodeProperty GetPropertyByName(string name) => _propertyCache.Values.First(x => x.Name == name);
}
