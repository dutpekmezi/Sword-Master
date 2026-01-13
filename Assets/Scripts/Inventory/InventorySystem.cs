using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class InventorySystem : BaseSystem
    {
        public static InventorySystem Instance { get; private set; }

        public InventorySystem()
        {
            Instance = this;
            OnInitialize();
        }

        protected override void OnInitialize()
        {
            SignalBus.Get<Entity.OnEntityDiedSignal>().Subscribe(HandleEntityDied);
        }

        protected override void OnDispose()
        {
            SignalBus.Get<Entity.OnEntityDiedSignal>().Unsubscribe(HandleEntityDied);
        }

        private void HandleEntityDied(Entity entity, Vector3 dropPosition)
        {
            if (entity == null)
                return;

            Inventory inventory = null;
            SignalBus.Get<Inventory.OnInventoryRequestSignal>()
                .Invoke(entity.gameObject, receivedInventory => inventory = receivedInventory);

            if (inventory == null)
                return;

            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                inventory.DropSlot(i, dropPosition, out _);
            }
        }
    }
}
