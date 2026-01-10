using UnityEngine;

namespace dutpekmezi
{
    public class InventorySlotComponent : MonoBehaviour
    {
        [SerializeField] private int initialSlotCount = 1;

        public InventorySlotSystem SlotSystem { get; private set; }

        private void Awake()
        {
            SlotSystem = new InventorySlotSystem(initialSlotCount);
        }

        private void OnDestroy()
        {
            SlotSystem?.Dispose();
            SlotSystem = null;
        }
    }
}
