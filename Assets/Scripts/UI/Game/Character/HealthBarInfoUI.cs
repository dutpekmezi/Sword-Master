using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils.Signal;
using UnityEngine.TextCore.Text;

namespace dutpekmezi
{
    public class HealthBarInfoUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image entityImage;

        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider abilityCooldownSlider;
        [SerializeField] private Slider levelSlider;

        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI levelText;

        private CharacterData characterData;
        private CharacterBase character;

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (character == null) return;

            UpdateAbilityCooldownSlider(character);
        }

        private void Init()
        {
            characterData = CharacterSystem.Instance.GetCurrentCharacterData();
            character = CharacterSystem.Instance.GetCurrentCharacter();

            if (characterData == null || character == null) return;

            SignalBus.Get<CharacterBase.OnStatsChange>().Subscribe(UpdateSliders);

            entityImage.sprite = characterData.Sprite;

            UpdateSliders(character);
        }

        private void UpdateSliders(CharacterBase character)
        {
            UpdateHealthSlider(character);
            UpdateAbilityCooldownSlider(character);
            UpdateLevelSlider(character);
        }

        private void UpdateHealthSlider(CharacterBase character)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = character.GetStatValue(StatType.MaxHealth);
            healthSlider.value = character.CurrentHealth;

            healthText.text = $"{(int)character.CurrentHealth} / {(int)character.GetStatValue(StatType.MaxHealth)}";
        }

        private void UpdateAbilityCooldownSlider(CharacterBase character)
        {
            float duration = character.AbilityCooldownDuration;
            float remaining = character.AbilityCooldownRemaining;

            if (duration <= 0f)
            {
                abilityCooldownSlider.minValue = 0;
                abilityCooldownSlider.maxValue = 1;
                abilityCooldownSlider.value = 1;
                return;
            }

            float elapsed = duration - remaining;
            abilityCooldownSlider.minValue = 0;
            abilityCooldownSlider.maxValue = duration;
            abilityCooldownSlider.value = Mathf.Clamp(elapsed, 0, duration);
        }

        private void UpdateLevelSlider(CharacterBase character)
        {
            levelSlider.minValue = 0;
            levelSlider.maxValue = character.GetStatValue(StatType.ExpToLevelUp);
            levelSlider.value = character.CurrentExp;

            levelText.text = $"{character.CurrentLevel}";
        }
    }
}
