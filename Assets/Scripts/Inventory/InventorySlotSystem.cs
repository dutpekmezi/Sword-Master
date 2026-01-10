using System;
using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    public class InventorySlotSystem : BaseSystem
    {
        [SerializeField] private int initialSlotCount;
        [SerializeField] private List<InventorySlot> slots;
        [SerializeReference] private object equippedItem;

        public static InventorySlotSystem Instance { get; private set; }
        public IReadOnlyList<InventorySlot> Slots => slots;
        public object EquippedItem => equippedItem;

        public InventorySlotSystem(int initialSlotCount = 1, List<InventorySlot> initialSlots = null)
        {
            Instance = this;
            this.initialSlotCount = Mathf.Max(0, initialSlotCount);
            slots = initialSlots ?? new List<InventorySlot>();
            OnInitialize();
        }

        protected override void OnInitialize()
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

        public int CreateSlot(object item = null)
        {
            var slot = new InventorySlot(item);
            slots.Add(slot);
            return slots.Count - 1;
        }

        public bool SetSlotItem(int slotIndex, object item)
        {
            if (!IsValidSlot(slotIndex))
                return false;

            slots[slotIndex].SetItem(item);
            return true;
        }

        public bool TryGetSlotItem(int slotIndex, out object item)
        {
            item = null;

            if (!IsValidSlot(slotIndex))
                return false;

            item = slots[slotIndex].Item;
            return true;
        }

        public bool DropSlot(int slotIndex, out object droppedItem)
        {
            droppedItem = null;

            if (!IsValidSlot(slotIndex))
                return false;

            droppedItem = slots[slotIndex].Clear();
            return droppedItem != null;
        }

        public bool DestroySlot(int slotIndex, bool destroyItem = true)
        {
            if (!IsValidSlot(slotIndex))
                return false;

            var item = slots[slotIndex].Item;
            slots.RemoveAt(slotIndex);

            if (!destroyItem || item == null)
                return true;

            if (item is UnityEngine.Object unityObject)
            {
                Destroy(unityObject);
                return true;
            }

            if (item is IDisposable disposable)
                disposable.Dispose();

            return true;
        }

        public bool EquipSlot(int slotIndex, bool removeFromSlot = false)
        {
            if (!IsValidSlot(slotIndex))
                return false;

            var slot = slots[slotIndex];
            if (!slot.HasItem)
                return false;

            equippedItem = slot.Item;

            if (removeFromSlot)
                slot.Clear();

            return true;
        }

        private bool IsValidSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < slots.Count;
        }
    }
}
