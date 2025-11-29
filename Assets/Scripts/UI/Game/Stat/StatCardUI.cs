using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils.Signal;
using DG.Tweening;

namespace dutpekmezi
{
    public class StatCardUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform cardTransform;

        [SerializeField] private Image frame;

        [SerializeField] private TextMeshProUGUI statValueText;
        [SerializeField] private TextMeshProUGUI statNameText;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration;
        [SerializeField] private float fadeValue;

        [Header("Floating Settings")]
        [SerializeField] private float floatingEndValue;
        [SerializeField] private float floatingDuration;

        private StatModifier statModifier;
        private StatType statType;

        public void Init(StatModifier modifier)
        {
            DOTween.Kill(frame);
            DOTween.Kill(cardTransform);

            this.statModifier = modifier;
            this.statType = modifier.Type;

            StatConfig statConfig = StatSystem.Instance.GetStatConfig(statType);

            statNameText.text = StatTypeExtensions.GetName(statType);
            frame.color = statConfig.Color;

            statValueText.text = FormatModifierValue(modifier);
            statValueText.color = statConfig.Color;

            LightAnim();
            WeaponFloatingAnim();
        }

        private string FormatModifierValue(StatModifier mod)
        {
            string sign = mod.Value >= 0 ? "+" : "";

            if (mod.Operation == ModifierOperation.PercentAdd || mod.Operation == ModifierOperation.PercentMultiply)
            {
                return $"{sign}{(mod.Value):F1}%";
            }
            else
            {
                return $"{sign}{mod.Value:F1}";
            }
        }
        private void LightAnim()
        {
            frame.DOFade(fadeValue, fadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void WeaponFloatingAnim()
        {
            cardTransform.DOAnchorPosY(floatingEndValue, floatingDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnClick()
        {
            SignalBus.Get<StatSystem.OnStatSelected>().Invoke(statModifier);
        }
    }
}