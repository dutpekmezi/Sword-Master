using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils.Signal;

namespace dutpekmezi
{
    public class WeaponHealthBarInfoUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image weaponImage;

        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider abilityCooldownSlider;
        [SerializeField] private Slider levelSlider;

        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI levelText;

        private WeaponData weaponData;
        private WeaponBase weapon;

        private void OnEnable()
        {
            SignalBus.Get<WeaponSystem.OnWeaponEquippedSignal>().Subscribe(OnWeaponEquipped);
            Init();
        }

        private void OnDisable()
        {
            SignalBus.Get<WeaponSystem.OnWeaponEquippedSignal>().Unsubscribe(OnWeaponEquipped);
            SignalBus.Get<WeaponBase.OnStatsChange>().Unsubscribe(UpdateSliders);
        }

        private void Update()
        {
            if (weapon == null)
            {
                weaponImage.enabled = false;
                return;
            }

            UpdateAbilityCooldownSlider(weapon);
        }

        private void Init()
        {
            weapon = WeaponSystem.Instance?.CurrentWeapon;
            weaponData = WeaponSystem.Instance?.CurrentWeaponData;

            if (weaponData == null || weapon == null) return;

            SubscribeToWeaponStats();
            SetWeaponImage(weaponData);
            UpdateSliders(weapon);
        }

        private void SubscribeToWeaponStats()
        {
            SignalBus.Get<WeaponBase.OnStatsChange>().Unsubscribe(UpdateSliders);
            SignalBus.Get<WeaponBase.OnStatsChange>().Subscribe(UpdateSliders);
        }

        private void OnWeaponEquipped(WeaponData data)
        {
            weapon = WeaponSystem.Instance.CurrentWeapon;
            weaponData = WeaponSystem.Instance.CurrentWeaponData ?? data;

            if (weaponData == null || weapon == null) return;

            SubscribeToWeaponStats();
            SetWeaponImage(weaponData);
            UpdateSliders(weapon);
        }

        private void SetWeaponImage(WeaponData data)
        {
            if (weaponImage == null) return;

            weaponImage.sprite = data.Icon != null ? data.Icon : data.Sprite;
        }

        private void UpdateSliders(WeaponBase currentWeapon)
        {
            weaponImage.enabled = true;

            UpdateHealthSlider(currentWeapon);
            UpdateAbilityCooldownSlider(currentWeapon);
            UpdateLevelSlider(currentWeapon);
        }

        private void UpdateHealthSlider(WeaponBase currentWeapon)
        {
            if (healthSlider == null || healthText == null) return;

            healthSlider.minValue = 0;
            healthSlider.maxValue = currentWeapon.GetStatValue(StatType.MaxHealth);
            healthSlider.value = currentWeapon.CurrentHealth;

            healthText.text = $"{(int)currentWeapon.CurrentHealth} / {(int)currentWeapon.GetStatValue(StatType.MaxHealth)}";
        }

        private void UpdateAbilityCooldownSlider(WeaponBase currentWeapon)
        {
            if (abilityCooldownSlider == null) return;

            float duration = currentWeapon.AbilityCooldownDuration;
            float remaining = currentWeapon.AbilityCooldownRemaining;

            if (duration <= 0f)
            {
                abilityCooldownSlider.minValue = 0;
                abilityCooldownSlider.maxValue = 1;
                abilityCooldownSlider.value = 1;
                return;
            }

            float elapsed = Mathf.Clamp(duration - remaining, 0, duration);
            abilityCooldownSlider.minValue = 0;
            abilityCooldownSlider.maxValue = duration;
            abilityCooldownSlider.value = elapsed;
        }

        private void UpdateLevelSlider(WeaponBase currentWeapon)
        {
            if (levelSlider == null || levelText == null) return;

            levelSlider.minValue = 0;
            levelSlider.maxValue = currentWeapon.GetStatValue(StatType.ExpToLevelUp);
            levelSlider.value = currentWeapon.CurrentExp;

            levelText.text = $"{currentWeapon.CurrentLevel}";
        }
    }
}
