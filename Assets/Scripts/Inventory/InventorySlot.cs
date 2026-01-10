using System;
using UnityEngine;

namespace dutpekmezi
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private GameObject item;

        public GameObject Item => item;
        public bool HasItem => item != null;

        public InventorySlot() { }

        public InventorySlot(GameObject initialItem)
        {
            item = initialItem;
        }

        public void SetItem(GameObject newItem)
        {
            item = newItem;
        }

        public GameObject Clear()
        {
            var removed = item;
            item = null;
            return removed;
        }
    }
}
