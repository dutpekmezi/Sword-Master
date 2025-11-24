using UnityEngine;
using Utils.LogicTimer;

namespace dutpekmezi
{
    public enum StatueType
    {
        Stat,
        Weapon
    }
    public abstract class StatueBase : MonoBehaviour
    {
        [SerializeField] protected float radius;
        [SerializeField] protected float requiredTime;

        private float updateTimer;

        public StatueType Type { get; private set; } 

        public void Init(StatueType type)
        {
            Type = type;
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (col.gameObject != CharacterSystem.Instance.GetCurrentCharacter().gameObject) return;

            updateTimer += LogicTimer.FixedDelta;

            if (updateTimer >= requiredTime)
            {
                GetUpgrade();
            }
        }

        protected virtual void GetUpgrade()
        {
            updateTimer = 0f;

            Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(this.gameObject);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.gameObject != CharacterSystem.Instance.GetCurrentCharacter().gameObject) return;

            updateTimer = 0f;
        }
    }
}