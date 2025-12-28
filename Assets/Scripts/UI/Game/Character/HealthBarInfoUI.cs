using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        [Header("Runtime")]
        [SerializeField] private Entity entity;

        private void Start()
        {
            InitializeEntity(entity ?? CharacterSystem.Instance?.GetCurrentCharacter());
        }

        private void Update()
        {
            if (entity == null) return;
            UpdateSliders(entity);
        }

        public void InitializeEntity(Entity targetEntity)
        {
            entity = targetEntity;

            if (entity == null) return;

            if (entity.EntityData != null)
            {
                entityImage.sprite = entity.EntityData.Sprite;
            }

            bool hasEnergy = entity is CharacterBase;
            if (energySlider != null)
                energySlider.gameObject.SetActive(hasEnergy);
            UpdateSliders(entity);
        }

        private void UpdateSliders(Entity targetEntity)
        {
            UpdateHealthSlider(targetEntity);
            UpdateEnergySlider(targetEntity as CharacterBase);
            UpdateLevelSlider(targetEntity);
        }

        private void UpdateHealthSlider(Entity targetEntity)
        {
            float maxHealth = targetEntity.GetStatValue(StatType.MaxHealth);
            if (maxHealth <= 0f) maxHealth = 1f;

            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = targetEntity.CurrentHealth;

            healthText.text = $"{(int)targetEntity.CurrentHealth} / {(int)maxHealth}";
        }

        private void UpdateEnergySlider(CharacterBase character)
        {
            if (energySlider == null) return;
            if (character == null) return;

            energySlider.minValue = 0;
            energySlider.maxValue = character.GetStatValue(StatType.Energy);
            energySlider.value = character.CurrentEnergy;
        }

        private void UpdateLevelSlider(Entity targetEntity)
        {
            float maxExp = targetEntity.GetStatValue(StatType.ExpToLevelUp);
            if (maxExp <= 0f) maxExp = 1f;

            levelSlider.minValue = 0;
            levelSlider.maxValue = maxExp;
            levelSlider.value = targetEntity.CurrentExp;

            levelText.text = $"{targetEntity.CurrentLevel}";
        }
    }
}
