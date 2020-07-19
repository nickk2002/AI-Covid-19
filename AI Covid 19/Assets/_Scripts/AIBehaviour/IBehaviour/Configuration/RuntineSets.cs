using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.IBehaviour.Configuration
{
    public abstract class RuntineSets<T> : ScriptableObject
    {
        public List<T> Items { get; set; }

        public void Clear()
        {
            Items.Clear();
        }

        public void Add(T item)
        {
            if (!Items.Contains(item))
                Items.Add(item);
        }

        public void Remove(T item)
        {
            if (Items.Contains(item))
                Items.Remove(item);
        }
    }
}