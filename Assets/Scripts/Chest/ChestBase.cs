using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class ChestBase : Entity
    {
        [Header("Assigned Data")]
        [SerializeField] private ChestData chestData;

        [Header("References")]
        [SerializeField] private Collider2D chestCollider;
        [SerializeField] private Animator chestAnimator;

        [SerializeField] private bool isOpened;

        public bool IsOpened => isOpened;

        public override void Initialize()
        {
            if (chestData == null)
            {
                Debug.LogError($"{name} is missing ChestData assignment.");
                return;
            }

            entityData = chestData;
            isOpened = false;

            base.Initialize();

            SignalBus.Get<OnTakeDamage>().Subscribe(OnTakeDamageHandler);
        }

        protected override void SetHealth(float amount)
        {
            if (isOpened) return;

            base.SetHealth(amount);
        }

        protected override void Die()
        {
            if (isOpened) return;

            isDead = true;
            OpenChest();
        }

        private void OpenChest()
        {
            isOpened = true;

            if (chestCollider != null)
                chestCollider.enabled = false;

            if (chestAnimator != null)
                chestAnimator.SetTrigger("Open");

            SignalBus.Get<ChestSystem.OnChestOpenedSignal>().Invoke(this);
        }

        public class OnTakeDamage : Signal<Entity, float> { }
    }
}
