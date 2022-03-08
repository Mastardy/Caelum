using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;using UnityEngine.PlayerLoop;

public class DynamicValueNetworked<T> : DynamicValue<T>, INetworkSerializable where T : unmanaged
{
    public void NetworkSerialize<TR>(BufferSerializer<TR> serializer) where TR : IReaderWriter
    {
        serializer.SerializeValue(ref max);
        serializer.SerializeValue(ref current);
    }
}