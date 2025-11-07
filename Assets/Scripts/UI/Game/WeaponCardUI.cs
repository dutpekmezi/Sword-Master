using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace dutpekmezi
{
    public class WeaponCardUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image frame;
        [SerializeField] private Image weaponImage;

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI detailText;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration;
        [SerializeField] private float fadeValue;

        private WeaponData weaponData;

        public void Init(WeaponData data)
        {
            weaponData = data;

            weaponImage.sprite = data.Icon;

            nameText.text = data.Name;
            detailText.text = data.Description;

            LightAnim();
        }

        private void LightAnim()
        {
            frame.DOFade(fadeValue, fadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}