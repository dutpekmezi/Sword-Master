using UnityEngine;

namespace dutpekmezi
{
    public class ExpDropParticle : Collectable
    {
        [Header("Collect Settings")]
        [SerializeField] private float collectStartDelay = 0.1f;
        [SerializeField] private float collectFlyDuration = 0.5f;

        protected override void Init()
        {
            base.Init();

            Collect();
        }

        public void Collect()
        {
            Collect(collectStartDelay, collectFlyDuration);
        }
    }
}
