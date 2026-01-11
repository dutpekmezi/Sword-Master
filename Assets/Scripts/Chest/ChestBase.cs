using DG.Tweening;
using Dutpekmezi.Services.PoolService;
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

        [Header("Damage Shake")]
        [SerializeField] private bool enableHitShake = true;
        [SerializeField] private float hitShakeDuration = 0.15f;
        [SerializeField] private Vector3 hitShakeStrength = new Vector3(0.1f, 0.1f, 0f);
        [SerializeField] private int hitShakeVibrato = 10;
        [SerializeField] private float hitShakeRandomness = 90f;
        [SerializeField] private bool hitShakeFadeOut = true;

        [SerializeField] private bool isOpened;

        private Tween hitShakeTween;

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

        protected override void TakeDamage(float damageAmount)
        {
            if (isOpened) return;

            PlayHitShake();
            base.TakeDamage(damageAmount);
        }

        protected override void Die()
        {
            if (isOpened) return;

            isDead = true;
            OpenChest();
            DropSlots();
            ObjectPoolManager.DeSpawn(gameObject);
        }

        private void DropSlots()
        {
            Inventory inventory = null;
            SignalBus.Get<Inventory.OnInventoryRequestSignal>()
                .Invoke(gameObject, receivedInventory => inventory = receivedInventory);
            if (inventory == null)
                return;

            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                inventory.DropSlot(i, transform.position, out _);
            }
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

        private void PlayHitShake()
        {
            if (!enableHitShake) return;

            if (hitShakeTween != null && hitShakeTween.IsActive())
            {
                hitShakeTween.Kill();
            }

            Vector3 originalLocalPosition = transform.localPosition;
            hitShakeTween = transform
                .DOShakePosition(
                    hitShakeDuration,
                    hitShakeStrength,
                    hitShakeVibrato,
                    hitShakeRandomness,
                    hitShakeFadeOut,
                    true)
                .OnKill(() => transform.localPosition = originalLocalPosition)
                .OnComplete(() => transform.localPosition = originalLocalPosition);
        }

        public class OnTakeDamage : Signal<Entity, float> { }
    }
}
