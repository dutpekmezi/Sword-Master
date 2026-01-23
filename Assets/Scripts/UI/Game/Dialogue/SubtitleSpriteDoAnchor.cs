using UnityEngine;
using UnityEngine.UI;

namespace dutpekmezi
{
    [RequireComponent(typeof(Image))]
    public class SubtitleSpriteDoAnchor : MonoBehaviour
    {
        [SerializeField] private DoAnim doAnim;
        [SerializeField] private Image subtitleImage;
        [SerializeField] private bool playOnEnable = true;

        private Sprite _lastSprite;

        private void Awake()
        {
            if (subtitleImage == null)
            {
                subtitleImage = GetComponent<Image>();
            }
        }

        private void OnEnable()
        {
            CacheSprite();

            if (playOnEnable && CurrentSprite() != null)
            {
                TriggerDoAnchor();
            }
        }

        private void Update()
        {
            Sprite current = CurrentSprite();
            if (current == _lastSprite) return;

            _lastSprite = current;
            if (current != null)
            {
                TriggerDoAnchor();
            }
        }

        private Sprite CurrentSprite()
        {
            if (subtitleImage == null) return null;
            return subtitleImage.overrideSprite != null ? subtitleImage.overrideSprite : subtitleImage.sprite;
        }

        private void CacheSprite()
        {
            _lastSprite = CurrentSprite();
        }

        private void TriggerDoAnchor()
        {
            if (doAnim == null)
            {
                doAnim = GetComponent<DoAnim>();
            }

            if (doAnim == null) return;

            doAnim.Play();
        }
    }
}
