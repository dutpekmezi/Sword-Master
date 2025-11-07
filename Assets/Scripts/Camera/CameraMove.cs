using UnityEngine;
using DG.Tweening;

namespace dutpekmezi
{
    public class CameraMove : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private float followDelay = 0.25f; // Delay for smooth follow feel
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        private Transform target;
        private Tween followTween;

        private void Start()
        {
            if (target == null)
            {
                return;
            }

            // Camera starts at target position
            transform.position = target.position + offset;
        }

        private void LateUpdate()
        {
            FollowCharacter();
        }

        private void FollowCharacter()
        {
            target = CharacterSystem.Instance.GetCurrentCharacterTransform();
            if (target == null) return;

            // Kill any previous tween to prevent stacking
            followTween?.Kill();

            Vector3 desiredPos = target.position + offset;

            // Smooth delayed movement toward the target
            followTween = transform.DOMove(desiredPos, followDelay)
                .SetEase(Ease.OutQuad)   // Smooth easing
                .SetSpeedBased(false);
        }
    }
}
