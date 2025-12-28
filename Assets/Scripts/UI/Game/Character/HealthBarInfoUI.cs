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
        [SerializeField] private Slider energySlider;
        [SerializeField] private Slider levelSlider;

        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI levelText;

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

            SignalBus.Get<CharacterBase.OnStatsChange>().Subscribe(UpdateSliders);

            entityImage.sprite = characterData.Sprite;

            UpdateSliders(character);
        }

        private void UpdateSliders(CharacterBase character)
        {
            UpdateHealthSlider(character);
            UpdateEnergySlider(character);
            UpdateLevelSlider(character);
        }

        private void UpdateHealthSlider(CharacterBase character)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = character.GetStatValue(StatType.MaxHealth);
            healthSlider.value = character.CurrentHealth;

            healthText.text = $"{(int)character.CurrentHealth} / {(int)character.GetStatValue(StatType.MaxHealth)}";
        }

        private void UpdateEnergySlider(CharacterBase character)
        {
            energySlider.minValue = 0;
            energySlider.maxValue = character.GetStatValue(StatType.Energy);
            energySlider.value = character.CurrentEnergy;
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