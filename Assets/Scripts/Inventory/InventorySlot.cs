using System;
using UnityEngine;

namespace dutpekmezi
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeReference] private object item;

        public object Item => item;
        public bool HasItem => item != null;

        public InventorySlot() { }

        public InventorySlot(object initialItem)
        {
            item = initialItem;
        }

        public void SetItem(object newItem)
        {
            item = newItem;
        }

        public object Clear()
        {
            var removed = item;
            item = null;
            return removed;
        }
    }
}
