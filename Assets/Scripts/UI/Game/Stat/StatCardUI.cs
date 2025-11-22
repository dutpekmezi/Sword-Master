using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils.Signal;

namespace dutpekmezi
{
    public class StatCardUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image statImage;

        [SerializeField] private TextMeshProUGUI statValueText;
        [SerializeField] private TextMeshProUGUI statNameText;

        private StatModifier statModifier;
        private StatType statType;

        public void Init(StatModifier modifier)
        {
            this.statModifier = modifier;
            this.statType = modifier.Type;

            StatConfig statConfig = StatSystem.Instance.GetStatConfig(statType);

            statImage.sprite = statConfig.Icon;
            statImage.color = statConfig.Color;

            statNameText.text = statType.ToString();
            statNameText.color = statConfig.Color;

            statValueText.text = FormatModifierValue(modifier);
            statValueText.color = statConfig.Color;
        }

        private string FormatModifierValue(StatModifier mod)
        {
            string sign = mod.Value >= 0 ? "+" : "";

            if (mod.Operation == ModifierOperation.PercentAdd || mod.Operation == ModifierOperation.PercentMultiply)
            {
                return $"{sign}{(mod.Value * 100):F1}%";
            }
            else
            {
                return $"{sign}{mod.Value:F1}";
            }
        }

        public void OnClick()
        {
            SignalBus.Get<StatSystem.OnStatSelected>().Invoke(statModifier);
        }
    }
}