using System.Collections;
using Dutpekmezi.Services.PoolService;
using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class Collectable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private StatModifier statModifier;
        private StatType statType;
        private Coroutine collectRoutine;
        private bool isInitialized;

        public StatModifier StatModifier => statModifier;
        public StatType StatType => statType;

        private void OnEnable()
        {
            Init();
        }

        protected virtual void Init()
        {
            var character = CharacterSystem.Instance?.GetCurrentCharacter();
            float scaleFactor = character != null ? character.CurrentLevel : 1f;

            statModifier = StatSystem.Instance.CreateRandomCollectableModifier(scaleFactor, this);
            statType = statModifier != null ? statModifier.Type : default;

            ApplyColor();
            isInitialized = true;
        }

        public void Collect(float startDelay, float flySpeed)
        {
            if (!isInitialized)
            {
                Init();
            }

            if (collectRoutine != null)
            {
                StopCoroutine(collectRoutine);
            }

            collectRoutine = StartCoroutine(CollectRoutine(startDelay, flySpeed));
        }

        protected virtual IEnumerator CollectRoutine(float startDelay, float flySpeed)
        {
            if (startDelay > 0f)
            {
                yield return new WaitForSeconds(startDelay);
            }

            var character = CharacterSystem.Instance?.GetCurrentCharacter();
            if (character == null)
            {
                collectRoutine = null;
                yield break;
            }

            if (flySpeed <= 0f)
            {
                transform.position = character.transform.position;
                OnCollected();
                yield break;
            }

            float threshold = 0.01f;
            while (true)
            {
                Vector3 targetPosition = character.transform.position;
                float step = flySpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                if ((transform.position - targetPosition).sqrMagnitude <= threshold * threshold)
                {
                    break;
                }

                yield return null;
            }

            transform.position = character.transform.position;
            OnCollected();
        }

        protected virtual void ApplyColor()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            StatConfig config = StatSystem.Instance.GetStatConfig(statType);
            spriteRenderer.color = config.Color;
        }

        protected virtual void OnCollected()
        {
            SignalBus.Get<StatSystem.OnStatSelected>().Invoke(statModifier);

            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.DeSpawn(gameObject);
            }

            collectRoutine = null;
        }
    }
}
