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

            SignalBus.Get<CharacterBase.OnStatsChange>().Subscribe(UpdateSliders);

            entityImage.sprite = characterData.Sprite;

            UpdateSliders(character);
        }

        private void UpdateSliders(CharacterBase character)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = character.GetStatValue(StatType.MaxHealth);
            healthSlider.value = character.CurrentHealth;

            healthText.text = $"{character.CurrentHealth} / {character.GetStatValue(StatType.MaxHealth)}";


            energySlider.minValue = 0;
            energySlider.maxValue = character.GetStatValue(StatType.Energy);
            energySlider.value = character.CurrentEnergy;
        }
    }
}