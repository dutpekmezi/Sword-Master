using UnityEngine;
using DG.Tweening;
using Utils.Signal;

namespace dutpekmezi
{
    public class CameraMove : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private float followDelay = 0.25f; // Delay for smooth follow feel
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Header("Weapon Influence")]
        [SerializeField] private float weaponPadding = 1.5f;
        [SerializeField] private float minOrthographicSize = 5f;
        [SerializeField] private float maxOrthographicSize = 18f;
        [SerializeField] private float minFollowDistance = 6f;
        [SerializeField] private float maxFollowDistance = 24f;

        private Transform target;
        private Tween followTween;
        private Tween sizeTween;
        private Camera cam;

        private float baseOrthographicSize;
        private float baseFollowDistance;
        private float lastOrbitRadius = -1f;

        private void Awake()
        {
            cam = GetComponent<Camera>();

            if (cam != null)
            {
                baseOrthographicSize = cam.orthographicSize;
            }

            baseFollowDistance = offset.magnitude;
        }

        private void OnEnable()
        {
            SignalBus.Get<WeaponSystem.OnWeaponEquippedSignal>().Subscribe(OnWeaponEquipped);
            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(OnStatSelected);
        }

        private void OnDisable()
        {
            SignalBus.Get<WeaponSystem.OnWeaponEquippedSignal>().UnSubscribe(OnWeaponEquipped);
            SignalBus.Get<StatSystem.OnStatSelected>().UnSubscribe(OnStatSelected);
        }

        private void Start()
        {
            if (target == null)
            {
                return;
            }

            // Camera starts at target position
            transform.position = target.position + offset;

            UpdateCameraRadius();
        }

        private void LateUpdate()
        {
            FollowCharacter();
            UpdateCameraRadius();
        }

        private void FollowCharacter()
        {
            target = CharacterSystem.Instance.GetCurrentCharacter().transform;
            if (target == null) return;

            // Kill any previous tween to prevent stacking
            followTween?.Kill();

            Vector3 desiredPos = target.position + offset;

            // Smooth delayed movement toward the target
            followTween = transform.DOMove(desiredPos, followDelay)
                .SetEase(Ease.OutQuad)   // Smooth easing
                .SetSpeedBased(false);
        }

        private float GetCurrentWeaponOrbitRadius()
        {
            var character = CharacterSystem.Instance.GetCurrentCharacter();
            if (character == null)
            {
                return 0f;
            }

            var weapon = character.GetComponentInChildren<WeaponBase>();
            if (weapon == null)
            {
                return 0f;
            }

            return weapon.GetStatValue(StatType.WeaponOrbitRadius);
        }

        private void UpdateCameraRadius()
        {
            float orbitRadius = GetCurrentWeaponOrbitRadius();

            if (Mathf.Approximately(orbitRadius, lastOrbitRadius))
            {
                return;
            }

            lastOrbitRadius = orbitRadius;

            float paddedRadius = orbitRadius + weaponPadding;

            if (cam != null && cam.orthographic)
            {
                float targetSize = Mathf.Clamp(baseOrthographicSize + paddedRadius, minOrthographicSize, maxOrthographicSize);

                sizeTween?.Kill();
                sizeTween = cam.DOOrthoSize(targetSize, followDelay)
                    .SetEase(Ease.OutQuad);
            }
            else
            {
                float targetDistance = Mathf.Clamp(baseFollowDistance + paddedRadius, minFollowDistance, maxFollowDistance);
                Vector3 desiredOffset = offset.normalized * targetDistance;

                followTween?.Kill();
                offset = desiredOffset;

                if (target != null)
                {
                    Vector3 desiredPos = target.position + offset;
                    followTween = transform.DOMove(desiredPos, followDelay)
                        .SetEase(Ease.OutQuad)
                        .SetSpeedBased(false);
                }
            }
        }

        private void OnWeaponEquipped(WeaponData weaponData)
        {
            lastOrbitRadius = -1f;
            UpdateCameraRadius();
        }

        private void OnStatSelected(StatModifier modifier)
        {
            if (modifier.Type == StatType.WeaponOrbitRadius)
            {
                lastOrbitRadius = -1f;
                UpdateCameraRadius();
            }
        }
    }
}
