using System;
using Unity.Netcode;

[Serializable]
public struct DynamicValueNetworked<T> : INetworkSerializable where T : unmanaged
{
    public T max;
    public T current;
    
    public void NetworkSerialize<TR>(BufferSerializer<TR> serializer) where TR : IReaderWriter
    {
        serializer.SerializeValue(ref max);
        serializer.SerializeValue(ref current);
    }
}