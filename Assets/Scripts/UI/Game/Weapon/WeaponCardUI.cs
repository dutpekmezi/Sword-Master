using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Utils.Signal;
using static dutpekmezi.WeaponSystem;

namespace dutpekmezi
{
    public class WeaponCardUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform weaponTransform;

        [SerializeField] private Image frame;
        [SerializeField] private Image weaponImage;

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI detailText;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration;
        [SerializeField] private float fadeValue;

        [Header("Floating Settings")]
        [SerializeField] private float floatingEndValue;
        [SerializeField] private float floatingDuration;

        private WeaponData weaponData;

        public void Init(WeaponData data)
        {
            DOTween.Kill(frame);
            DOTween.Kill(weaponTransform);

            weaponData = data;

            weaponTransform.localPosition = Vector3.zero;

            frame.color = new Color(1, 1, 1, 0);
            weaponImage.sprite = data.Icon;

            nameText.text = data.Name;
            detailText.text = data.Description;

            LightAnim();
            WeaponFloatingAnim();
        }

        public void OnClick()
        {
            SignalBus.Get<OnWeaponSelected>().Invoke(weaponData);
        }

        private void LightAnim()
        {
            frame.DOFade(fadeValue, fadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void WeaponFloatingAnim()
        {
            weaponTransform.DOAnchorPosY(floatingEndValue, floatingDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private WeaponData GetWeapon()
        {
            DOTween.Kill(frame);

            return weaponData;
        }

        private void OnDisable()
        {
            DOTween.Kill(frame);
            DOTween.Kill(weaponTransform);
        }
    }
}