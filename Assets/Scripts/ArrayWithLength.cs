using System;
using System.Reflection;

public class ArrayWithLength<T>
{
    public T[] array ;
    public int length;

    public ArrayWithLength(T[] array, int length)
    {
        this.array = array;
        this.length = length;
    }

    public void forEach(Action<T> callback) {
        for (int i = 0; i < length; i++) callback(array[i]);
    }
}