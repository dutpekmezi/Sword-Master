using DG.Tweening;
using Dutpekmezi.Services.PoolService;
using System.Collections;
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
        [SerializeField] private GameObject chestCellFructuredPrefab;
        [SerializeField] private SpriteRenderer chestSpriteRenderer;

        [Header("Fractured Spawn")]
        [SerializeField] private float fracturedPieceForce = 3f;
        [SerializeField] private float fracturedPieceTorque = 10f;

        [Header("Damage Shake")]
        [SerializeField] private bool enableHitShake = true;
        [SerializeField] private float hitShakeDuration = 0.15f;
        [SerializeField] private Vector3 hitShakeStrength = new Vector3(0.1f, 0.1f, 0f);
        [SerializeField] private int hitShakeVibrato = 10;
        [SerializeField] private float hitShakeRandomness = 90f;
        [SerializeField] private bool hitShakeFadeOut = true;

        [Header("Despawn")]
        [SerializeField] private float chestDespawnDelay = 0f;

        [SerializeField] private bool isOpened;

        private Tween hitShakeTween;

        private GameObject fracturedChest;

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
            EnableSpriteRenderer(false);
            SpawnFracturedChest();
            ScheduleChestDespawn();
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

            SignalBus.Get<ChestSystem.OnChestOpenedSignal>().Invoke(this);
        }

        private void EnableSpriteRenderer(bool value = true)
        {
            if (chestSpriteRenderer != null)
            {
                chestSpriteRenderer.enabled = value;
            }
        }

        private void SpawnFracturedChest()
        {
            if (chestCellFructuredPrefab == null) return;

            var fracturedInstance = ObjectPoolManager.SpawnObject(chestCellFructuredPrefab, this.transform.position);
            fracturedInstance.transform.position = this.transform.position;

            fracturedChest = fracturedInstance;

            if (fracturedInstance == null) return;

            ApplyFracturedForces(fracturedInstance);
        }

        private void ScheduleChestDespawn()
        {
            if (chestDespawnDelay <= 0f)
            {
                ObjectPoolManager.DeSpawn(fracturedChest);
                ObjectPoolManager.DeSpawn(gameObject);
                return;
            }

            StartCoroutine(DespawnChestAfterDelay(chestDespawnDelay));
        }

        private IEnumerator DespawnChestAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            ObjectPoolManager.DeSpawn(fracturedChest);
            ObjectPoolManager.DeSpawn(gameObject);
        }

        private void ApplyFracturedForces(GameObject fracturedInstance)
        {
            var rigidbodies = fracturedInstance.GetComponentsInChildren<Rigidbody2D>();
            foreach (var body in rigidbodies)
            {
                var direction = Random.insideUnitCircle.normalized;
                body.AddForce(direction * fracturedPieceForce, ForceMode2D.Impulse);

                if (fracturedPieceTorque != 0f)
                {
                    var torque = Random.Range(-fracturedPieceTorque, fracturedPieceTorque);
                    body.AddTorque(torque, ForceMode2D.Impulse);
                }
            }
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
