namespace GraphData;

public readonly record struct Connection
{
    public readonly ulong NodeMask;
    public readonly ulong PropertyMask;

    public uint SourceNodeId => (uint)(NodeMask >> 32);
    public uint TargetNodeId => (uint)(NodeMask & uint.MaxValue);

    public uint SourcePropertyId => (uint)(PropertyMask >> 32);
    public uint TargetPropertyId => (uint)(PropertyMask & uint.MaxValue);

    public Connection(uint sourceNodeId, uint sourcePropId, uint targetNodeId, uint targetPropId)
    {
        NodeMask = (ulong)sourceNodeId << 32 | targetNodeId;
        PropertyMask = (ulong)sourcePropId << 32 | targetPropId;
    }
}
