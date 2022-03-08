using System;

[Serializable]
public struct DynamicValue<T> where T : unmanaged
{
    public T max;
    public T current;
}