using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int initialSlotCount = 1;
        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

        public IReadOnlyList<InventorySlot> Slots => slots;

        private void Awake()
        {
            EnsureSlotCount(initialSlotCount);
        }

        public void EnsureSlotCount(int count)
        {
            if (count < 0)
                count = 0;

            while (slots.Count < count)
            {
                slots.Add(new InventorySlot());
            }
        }

        public int CreateSlot(GameObject item = null)
        {
            var slot = new InventorySlot(item);
            slots.Add(slot);
            return slots.Count - 1;
        }

        public bool SetSlotItem(int slotIndex, GameObject item)
        {
            if (!IsValidSlot(slotIndex))
                return false;

            slots[slotIndex].SetItem(item);
            return true;
        }

        public bool TryGetSlotItem(int slotIndex, out GameObject item)
        {
            item = null;

            if (!IsValidSlot(slotIndex))
                return false;

            item = slots[slotIndex].Item;
            return true;
        }

        public bool ClearSlot(int slotIndex, out GameObject removedItem)
        {
            removedItem = null;

            if (!IsValidSlot(slotIndex))
                return false;

            removedItem = slots[slotIndex].Clear();
            return removedItem != null;
        }

        private bool IsValidSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < slots.Count;
        }
    }
}
