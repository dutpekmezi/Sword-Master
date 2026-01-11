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

        private void Awake()
        {
            EnsureSlotCount(initialSlotCount);
        }

        private void OnEnable()
        {
            SignalBus.Get<OnInventoryRequestSignal>().Subscribe(HandleInventoryRequest);
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

        public class OnInventoryRequestSignal : Signal<GameObject, Action<Inventory>> { }
    }
}
