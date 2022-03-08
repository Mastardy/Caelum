using System;
using Unity.Netcode;

[Serializable]
public class DynamicValueNetworked<T> : DynamicValue<T>, INetworkSerializable where T : unmanaged
{
    public void NetworkSerialize<TR>(BufferSerializer<TR> serializer) where TR : IReaderWriter
    {
        serializer.SerializeValue(ref max);
        serializer.SerializeValue(ref current);
    }
}