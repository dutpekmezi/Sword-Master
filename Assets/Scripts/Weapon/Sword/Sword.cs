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
        [SerializeField] private SlashObj slash;
        [SerializeField] private float slashObjScaleX;
        [SerializeField] private float slashObjScaleY;
        [SerializeField] private float slashObjScaleDuration;
        [SerializeField] private float slashObjDeScaleDuration;

        private bool isMoving = false;
        private float targetAngle;
        private Vector2 targetPosition;


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
