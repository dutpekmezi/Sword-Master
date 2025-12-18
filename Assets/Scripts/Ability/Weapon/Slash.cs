using DG.Tweening;
using DG.Tweening.Core.Easing;
using Dutpekmezi.Services.PoolService;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "Slash", menuName = "Game/Scriptable Objects/Ability/Weapon/Slash")]
    public class Slash : AbilityBase<WeaponBase>
    {
        [Header("Ability Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private SlashObj slash;
        [SerializeField] private float slashObjScaleX;
        [SerializeField] private float slashObjScaleY;
        [SerializeField] private float slashObjScaleDuration;
        [SerializeField] private float slashObjDeScaleDuration;

        private WeaponBase weapon;

        private bool isMoving = false;
        private float targetAngle;
        private Vector2 targetPosition;

        protected override void ExecuteAbility(WeaponBase weapon)
        {
            this.weapon = weapon;

            if (/*Input.GetMouseButtonDown(0) && */!isMoving)
            {
                var character = CharacterSystem.Instance.GetCurrentCharacter();

                targetAngle = weapon.currentAngle + 180f;
                if (targetAngle > 360f)
                    targetAngle -= 360f;

                targetPosition = character.transform.position + new Vector3(
                    Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                    Mathf.Sin(targetAngle * Mathf.Deg2Rad),
                    0f
                ) * weapon.GetStatValue(StatType.WeaponOrbitRadius);

                isMoving = true;
                weapon.SetRotate(false);
                MoveToOpposite();
            }
        }

        private void MoveToOpposite()
        {
            float z = weapon.transform.position.z;
            weapon.transform.DOMove(new Vector3(targetPosition.x, targetPosition.y, z),
                                         Vector2.Distance(weapon.transform.position, targetPosition) / (weapon.GetStatValue(StatType.WeaponOrbitSpeed) * 5))
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                    Vector2 dir = (targetPosition - (Vector2)weapon.transform.position).normalized;
                    if (dir != Vector2.zero)
                        weapon.transform.up = dir;
                })
                .OnStart(() =>
                {
                    DOVirtual.DelayedCall(0.05f, Slashing);
                })
            .OnComplete(() =>
            {
                weapon.transform.position = new Vector3(targetPosition.x, targetPosition.y, z);
                weapon.currentAngle = targetAngle;
                isMoving = false;
                weapon.SetRotate(true);

                DOTween.Kill(weapon.transform);
            });
        }


        private void Slashing()
        {
            var slashObj = ObjectPoolManager.SpawnObject(
                slash, CharacterSystem.Instance.GetCurrentCharacter().transform.position);
            slashObj.transform.localScale = Vector2.zero;

            slashObj.transform.rotation = weapon.transform.rotation;

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

        protected override bool CanUse(WeaponBase weapon)
        {
            return base.CanUse(weapon);
        }
    }
}