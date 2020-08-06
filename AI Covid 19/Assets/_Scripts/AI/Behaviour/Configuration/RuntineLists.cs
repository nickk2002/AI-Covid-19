using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AI.Behaviour.Configuration
{
    public abstract class RuntineLists<T> : ScriptableObject
    {
        [HideInInspector] public List<T> items;

        public object this[int i] => items[i];
        public void Clear()
        {
            items.Clear();
        }

        public int Count => items.Count;
        public void Add(T item)
        {
            if (!items.Contains(item))
                items.Add(item);
        }

        public void Remove(T item)
        {
            if (items.Contains(item))
                items.Remove(item);
        }
    }
}