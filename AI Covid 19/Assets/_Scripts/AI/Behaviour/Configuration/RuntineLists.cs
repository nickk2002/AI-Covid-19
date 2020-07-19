using System.Collections.Generic;
using Covid19.Utils;
using UnityEngine;

namespace Covid19.AI.Behaviour.Configuration
{
    public abstract class RuntineLists<T> : ScriptableObject
    {
        [HideInInspector] public List<T> items;

        public void Clear()
        {
            items.Clear();
        }

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