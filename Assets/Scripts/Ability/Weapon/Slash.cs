using DG.Tweening;
using Dutpekmezi.Services.PoolService;
using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "Slash", menuName = "Game/Scriptable Objects/Ability/Weapon/Slash")]
    public class Slash : AbilityBase
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

        protected override void ExecuteAbility(Entity owner)
        {
            if (owner is not WeaponBase weapon)
                return;

            this.weapon = weapon;

            if (!isMoving)
            {
                var character = CharacterSystem.Instance.GetCurrentCharacter();

                targetAngle = weapon.currentAngle + 180f;
                if (targetAngle > 360f) targetAngle -= 360f;

                targetPosition = (Vector2)character.transform.position + new Vector2(
                    Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                    Mathf.Sin(targetAngle * Mathf.Deg2Rad)
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
                if (dir != Vector2.zero) weapon.transform.up = dir;
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
            var slashObj = ObjectPoolManager.SpawnObject(slash, CharacterSystem.Instance.GetCurrentCharacter().transform.position);

            SlashSettings settings = new SlashSettings
            {
                ScaleX = slashObjScaleX,
                ScaleY = slashObjScaleY,
                ScaleDuration = slashObjScaleDuration,
                DeScaleDuration = slashObjDeScaleDuration,
                Rotation = weapon.transform.rotation
            };

            slashObj.Init(settings);
        }

        protected override bool CanUse(Entity owner)
        {
            if (owner is not WeaponBase weapon)
                return false;

            return base.CanUse(weapon);
        }
    }
    public struct SlashSettings
    {
        public float ScaleX;
        public float ScaleY;
        public float ScaleDuration;
        public float DeScaleDuration;
        public Quaternion Rotation;
    }
}
