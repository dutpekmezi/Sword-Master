using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Utils.Signal;

namespace dutpekmezi
{
    public class Sword : WeaponBase
    {
        [Header("Ability Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Slash slash;
        [SerializeField] private float slashObjScaleX;
        [SerializeField] private float slashObjScaleY;
        [SerializeField] private float slashObjScaleDuration;
        [SerializeField] private float slashObjDeScaleDuration;

        private bool isMoving = false;
        private float targetAngle;
        private Vector2 targetPosition;

        protected override void Ability()
        {
            if (Input.GetMouseButtonDown(0) && !isMoving)
            {
                var character = CharacterSystem.Instance.GetCurrentCharacter();

                targetAngle = currentAngle + 180f;
                if (targetAngle > 360f)
                    targetAngle -= 360f;

                targetPosition = character.transform.position + new Vector3(
                    Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                    Mathf.Sin(targetAngle * Mathf.Deg2Rad),
                    0f
                ) * character.GetStatValue(StatType.WeaponOrbitRadius);

                isMoving = true;
                SetRotate(false);
                MoveToOpposite(character);
            }
        }

        private void MoveToOpposite(CharacterBase character)
        {
            float z = transform.position.z;

            transform.DOMove(new Vector3(targetPosition.x, targetPosition.y, z),
                                         Vector2.Distance(transform.position, targetPosition) / (character.GetStatValue(StatType.WeaponOrbitSpeed) * 0.2f))
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
                    if (dir != Vector2.zero)
                        transform.up = dir;
                })
                .OnStart(() =>
                {
                    DOVirtual.DelayedCall(0.05f, Slash);
                })
                .OnComplete(() =>
                {
                    transform.position = new Vector3(targetPosition.x, targetPosition.y, z);
                    currentAngle = targetAngle;
                    isMoving = false;
                    SetRotate(true);

                    DOTween.Kill(transform);
                });
        }


        private void Slash()
        {
            var slashObj = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(
                slash, CharacterSystem.Instance.GetCurrentCharacter().transform.position);

            slashObj.transform.localScale = Vector2.zero;

            slashObj.transform.rotation = transform.rotation;

            slashObj.transform.DOScale(new Vector2(slashObjScaleX, slashObjScaleY), slashObjScaleDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    slashObj.transform.DOScale(Vector2.zero, slashObjDeScaleDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            slashObj.transform.rotation = Quaternion.identity;
                            DOTween.Kill(slashObj.transform);
                            Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(slashObj.gameObject);
                        });
                });
        }


        private void OnTriggerEnter2D(Collider2D col)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                SignalBus.Get<EnemyBase.OnTakeDamage>().Invoke(enemy, weaponData.AttackDamage);
            }
        }
    }
}
