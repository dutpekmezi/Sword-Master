using UnityEngine;
using Utils.LogicTimer;

namespace dutpekmezi
{
    public enum StatueType
    {
        Stat,
        Weapon
    }
    public class StatueBase : MonoBehaviour
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
            if (col != CharacterSystem.Instance.GetCurrentCharacter()) return;

            updateTimer += LogicTimer.FixedDelta;

            if (updateTimer >= requiredTime)
            {
                GetUpgrade();
                gameObject.SetActive(false);
            }
        }

        public void GetUpgrade()
        {
            updateTimer = 0f;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col != CharacterSystem.Instance.GetCurrentCharacter()) return;

            updateTimer = 0f;
        }
    }
}