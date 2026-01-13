using UnityEngine;

namespace dutpekmezi
{
    public class RandomDropParticle : Collectable
    {
        [Header("Drop Settings")]
        [SerializeField] private bool useRandomStat = true;
        [SerializeField] private StatType selectedStat;
        [SerializeField] [Min(0f)] private float scaleMultiplier = 1f;
        [SerializeField] [Range(0f, 1f)] private float dropChance = 1f;

        [Header("Collect Settings")]
        [SerializeField] private float collectStartDelay = 0.1f;
        [SerializeField] private float collectFlySpeed = 3f;

        protected override void Init()
        {
            if (!ShouldDrop())
            {
                Despawn();
                return;
            }

            base.Init();

            Collect();
        }

        protected override StatModifier CreateCollectableModifier(float scaleFactor)
        {
            float scaledFactor = scaleFactor * scaleMultiplier;

            if (useRandomStat)
            {
                return StatSystem.Instance.CreateRandomCollectableModifier(scaledFactor, this);
            }

            return StatSystem.Instance.CreateRandomModifier(selectedStat, scaledFactor, this);
        }

        public void Collect()
        {
            Collect(collectStartDelay, collectFlySpeed);
        }

        private bool ShouldDrop()
        {
            if (dropChance >= 1f)
            {
                return true;
            }

            return Random.value <= dropChance;
        }

        private void Despawn()
        {
            if (Dutpekmezi.Services.PoolService.ObjectPoolManager.Instance != null)
            {
                Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(gameObject);
                return;
            }

            gameObject.SetActive(false);
        }
    }
}
