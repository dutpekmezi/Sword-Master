using UnityEngine;

namespace dutpekmezi
{
    public class RandomDropParticle : Collectable
    {
        [Header("Collect Settings")]
        [SerializeField] private float collectStartDelay = 0.1f;
        [SerializeField] private float collectFlySpeed = 3f;

        protected override void Init()
        {
            base.Init();

            Collect();
        }

        public void Collect()
        {
            Collect(collectStartDelay, collectFlySpeed);
        }
    }
}
