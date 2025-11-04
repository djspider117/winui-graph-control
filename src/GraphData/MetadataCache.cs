using System.Collections.Frozen;
using System.Xml.Linq;

namespace GraphData;

public static class MetadataCache
{
    private static IDictionary<uint, NodeProperty> _propertyCache = new Dictionary<uint, NodeProperty>();
    private static IDictionary<uint, NodeTypeDefinition> _nodeCache = new Dictionary<uint, NodeTypeDefinition>();
    private static IDictionary<string, NodeTypeDefinition> _nodeStringCache = new Dictionary<string, NodeTypeDefinition>();

    private static readonly Lock _lock = new();
    private static uint _globalIdCounter;

    public static void Freeze()
    {
        lock (_lock)
        {
            if (_propertyCache != null)
                _propertyCache = _propertyCache.ToFrozenDictionary(x => x.Key, x => x.Value);

            if (_nodeCache != null)
                _nodeCache = _nodeCache.ToFrozenDictionary(x => x.Key, x => x.Value);
        }
    }

    public static NodeTypeDefinition GetOrRegisterTypeDefinition(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (_nodeStringCache.TryGetValue(name, out var existing))
            return existing;

        var id = GetNextId();
        var node = new NodeTypeDefinition(id, name);
        _nodeCache.Add(node.TypeId, node);
        _nodeStringCache.Add(name, node);
        return node;
    }

    public static NodeProperty RegisterInputPropertyForType(uint nodeType, string name, uint propTypeId)
    {
        if (!_nodeCache.TryGetValue(nodeType, out var existing))
            throw new InvalidOperationException("Not found");

        var prop = new NodeProperty(GetNextId(), name, propTypeId);
        existing.InputProperties.Add(prop);

        _propertyCache.Add(prop.PropertyId, prop);

        return prop;
    }
    public static NodeProperty RegisterOutputPropertyForType(uint nodeType, string name, uint propTypeId)
    {
        if (!_nodeCache.TryGetValue(nodeType, out var existing))
            throw new InvalidOperationException("Not found");

        var prop = new NodeProperty(GetNextId(), name, propTypeId);
        existing.OutputProperties.Add(prop);

        _propertyCache.Add(prop.PropertyId, prop);

        return prop;
    }

    public static uint GetNextId() => ++_globalIdCounter;

    public static NodeProperty GetProperty(uint propType)
    {
        if (_propertyCache.TryGetValue(propType, out var existing))
            return existing;

        throw new InvalidOperationException("Not found");
    }
    public static NodeProperty GetInputProperty(uint nodeType, string propName)
    {
        if (!_nodeCache.TryGetValue(nodeType, out var node))
            throw new InvalidOperationException("Not found");

        return node.InputProperties.Find(x => x.Name == propName);
    }
    public static NodeProperty GetInputProperty(string nodeTypeName, string propName)
    {
        if (!_nodeStringCache.TryGetValue(nodeTypeName, out var node))
            throw new InvalidOperationException("Not found");

        return node.InputProperties.Find(x => x.Name == propName);
    }

    public static NodeProperty GetOutputProperty(uint nodeType, string propName)
    {
        if (!_nodeCache.TryGetValue(nodeType, out var node))
            throw new InvalidOperationException("Not found");

        return node.OutputProperties.Find(x => x.Name == propName);
    }
    public static NodeProperty GetOutputProperty(string nodeTypeName, string propName)
    {
        if (!_nodeStringCache.TryGetValue(nodeTypeName, out var node))
            throw new InvalidOperationException("Not found");

        return node.OutputProperties.Find(x => x.Name == propName);
    }
}

public static class NodeTypeDefExtensions
{
    public static NodeProperty GetInputProperty(this Node node, string propName)
    {
        return MetadataCache.GetInputProperty(node.TypeDefinition.TypeId, propName);
    }

    public static NodeProperty GetOutputProperty(this Node node, string propName)
    {
        return MetadataCache.GetOutputProperty(node.TypeDefinition.TypeId, propName);
    }
}
