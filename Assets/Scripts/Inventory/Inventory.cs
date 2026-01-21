using System;
using System.Collections.Generic;
using Dutpekmezi.Services.PoolService;
using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int initialSlotCount = 1;
        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

        public IReadOnlyList<InventorySlot> Slots => slots;
        private readonly List<GameObject> initialItems = new List<GameObject>();
        private int initialSlotsSnapshot;

        private void Awake()
        {
            EnsureSlotCount(initialSlotCount);
            CacheInitialSlots();
        }

        private void OnEnable()
        {
            SignalBus.Get<OnInventoryRequestSignal>().Subscribe(HandleInventoryRequest);
            ResetSlotsToInitialItems();
        }

        private void OnDisable()
        {
            SignalBus.Get<OnInventoryRequestSignal>().Unsubscribe(HandleInventoryRequest);
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

        public bool DropSlot(int slotIndex, Vector3 dropPosition, out GameObject droppedItem)
        {
            droppedItem = null;

            if (!IsValidSlot(slotIndex))
                return false;

            var itemPrefab = slots[slotIndex].Clear();
            if (itemPrefab == null)
                return false;

            droppedItem = ObjectPoolManager.SpawnObject(itemPrefab, dropPosition);
            return droppedItem != null;
        }

        private bool IsValidSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < slots.Count;
        }

        private void HandleInventoryRequest(GameObject owner, Action<Inventory> callback)
        {
            if (owner != gameObject)
                return;

            callback?.Invoke(this);
        }

        private void CacheInitialSlots()
        {
            initialItems.Clear();
            initialSlotsSnapshot = slots.Count;

            foreach (var slot in slots)
            {
                initialItems.Add(slot.Item);
            }
        }

        private void ResetSlotsToInitialItems()
        {
            if (initialItems.Count == 0 && initialSlotsSnapshot == 0)
                return;

            if (slots.Count != initialItems.Count)
            {
                slots.Clear();
                for (int i = 0; i < initialItems.Count; i++)
                {
                    slots.Add(new InventorySlot(initialItems[i]));
                }

                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].SetItem(initialItems[i]);
            }
        }

        public class OnInventoryRequestSignal : Signal<GameObject, Action<Inventory>> { }
    }
}
