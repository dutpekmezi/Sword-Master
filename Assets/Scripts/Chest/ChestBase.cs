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
        [SerializeField] private float fracturedDespawnDelay = 2f;

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
            EnableSpriteRenderer();
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

        private void EnableSpriteRenderer()
        {
            if (chestSpriteRenderer == null)
            {
                chestSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (chestSpriteRenderer != null)
            {
                chestSpriteRenderer.enabled = true;
            }
        }

        private void SpawnFracturedChest()
        {
            if (chestCellFructuredPrefab == null) return;

            var fracturedInstance = ObjectPoolManager.SpawnObject(chestCellFructuredPrefab, transform.position);
            if (fracturedInstance == null) return;

            ApplyFracturedForces(fracturedInstance);
            ScheduleFracturedDespawn(fracturedInstance);
        }

        private void ScheduleChestDespawn()
        {
            if (chestDespawnDelay <= 0f)
            {
                ObjectPoolManager.DeSpawn(gameObject);
                return;
            }

            StartCoroutine(DespawnChestAfterDelay(chestDespawnDelay));
        }

        private IEnumerator DespawnChestAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

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

        private void ScheduleFracturedDespawn(GameObject fracturedInstance)
        {
            if (fracturedDespawnDelay <= 0f)
            {
                ObjectPoolManager.DeSpawn(fracturedInstance);
                return;
            }

            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.StartCoroutine(DespawnFracturedAfterDelay(fracturedInstance, fracturedDespawnDelay));
            }
            else
            {
                ObjectPoolManager.DeSpawn(fracturedInstance);
            }
        }

        private static IEnumerator DespawnFracturedAfterDelay(GameObject fracturedInstance, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (fracturedInstance != null)
            {
                ObjectPoolManager.DeSpawn(fracturedInstance);
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
