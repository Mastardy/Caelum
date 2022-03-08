using System;

[Serializable]
public class DynamicValue<T> where T : unmanaged
{
    public T max;
    public T current;
}