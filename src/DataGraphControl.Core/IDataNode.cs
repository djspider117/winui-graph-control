namespace DataGraphControl.Core;

public interface IDataNode<TData> : INode
{
    TData Data { get; }
}
