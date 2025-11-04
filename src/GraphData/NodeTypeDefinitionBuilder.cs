using System.Numerics;

namespace GraphData;

public class NodeTypeDefinitionBuilder
{
    private readonly NodeTypeDefinition _curNode;
    private bool _built = false;

    private NodeTypeDefinitionBuilder(uint id, string name)
    {
        _curNode = new NodeTypeDefinition(id, name);
    }
    private NodeTypeDefinitionBuilder(NodeTypeDefinition node) => _curNode = node;

    public static NodeTypeDefinitionBuilder Create(uint id, string name)
    {
        return new NodeTypeDefinitionBuilder(id, name);
    }

    public static NodeTypeDefinitionBuilder Create(string name)
    {
        var node = MetadataCache.GetOrRegisterTypeDefinition(name);
        return new NodeTypeDefinitionBuilder(node);
    }

    public NodeTypeDefinitionBuilder WithInputProperty(string propName, KnownPropertyType typeId) => WithInputProperty(propName, (uint)typeId);
    public NodeTypeDefinitionBuilder WithOutputProperty(string propName, KnownPropertyType typeId) => WithOutputProperty(propName, (uint)typeId);

    public NodeTypeDefinitionBuilder WithInputProperty(string propName, uint typeId)
    {
        MetadataCache.RegisterInputPropertyForType(_curNode.TypeId, propName, typeId);
        return this;
    }
    public NodeTypeDefinitionBuilder WithOutputProperty(string propName, uint typeId)
    {
        MetadataCache.RegisterOutputPropertyForType(_curNode.TypeId, propName, typeId);
        return this;
    }

    public NodeTypeDefinition Build()
    {
        if (_built)
            throw new InvalidOperationException("Node already built");

        _built = true;
        return _curNode;
    }
}