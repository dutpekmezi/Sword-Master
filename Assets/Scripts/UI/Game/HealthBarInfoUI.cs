using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace dutpekmezi
{
    public class HealthBarInfoUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image characterImage;

        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider energySlider;

        [SerializeField] private TextMeshProUGUI healthText;

        private CharacterData characterData;
        private CharacterBase character;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            characterData = CharacterSystem.Instance.GetCurrentCharacterData();
            character = CharacterSystem.Instance.GetCurrentCharacter();

            if (characterData == null || character == null) return;

            character.OnStatsChange += UpdateSliders;

            characterImage.sprite = characterData.Icon;

            UpdateSliders(character);
        }

        private void UpdateSliders(CharacterBase character)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = characterData.MaxHealth;
            healthSlider.value = character.CurrentHealth;

            healthText.text = $"{character.CurrentHealth} / {characterData.MaxHealth}";
        }
    }
}