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

        public virtual void Init()
        {
            var character = CharacterSystem.Instance?.GetCurrentCharacter();
            float scaleFactor = character != null ? character.CurrentLevel : 1f;

            statModifier = StatSystem.Instance.CreateRandomCollectableModifier(scaleFactor, this);
            statType = statModifier != null ? statModifier.Type : default;

            ApplyColor();
            isInitialized = true;
        }

        public void Collect(float startDelay, float flyDuration)
        {
            if (!isInitialized)
            {
                Init();
            }

            if (collectRoutine != null)
            {
                StopCoroutine(collectRoutine);
            }

            collectRoutine = StartCoroutine(CollectRoutine(startDelay, flyDuration));
        }

        protected virtual IEnumerator CollectRoutine(float startDelay, float flyDuration)
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

            if (flyDuration <= 0f)
            {
                transform.position = character.transform.position;
                OnCollected();
                yield break;
            }

            float elapsed = 0f;
            Vector3 startPosition = transform.position;

            while (elapsed < flyDuration)
            {
                elapsed += Time.deltaTime;

                Vector3 targetPosition = character.transform.position;
                float t = Mathf.Clamp01(elapsed / flyDuration);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                yield return null;
            }

            transform.position = character.transform.position;
            OnCollected();
        }

        protected virtual void ApplyColor()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

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
            else
            {
                Destroy(gameObject);
            }

            collectRoutine = null;
        }
    }
}
