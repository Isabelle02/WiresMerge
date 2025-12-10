using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedDictionary<TKey, TValue>
{
    [SerializeField] private List<SerializedDictionaryElement<TKey, TValue>> _elements;

    public TValue this[TKey key] => _elements.Find(e => key.Equals(e.Key)).Value;
}

[Serializable]
public class SerializedDictionaryElement<TKey, TValue>
{
    [SerializeField] private TKey _key;
    [SerializeField] private TValue _value;

    public TKey Key => _key;
    public TValue Value => _value;
}
