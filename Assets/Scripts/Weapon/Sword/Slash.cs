using UnityEngine;
using DG.Tweening;
using Utils.Signal;

namespace dutpekmezi
{
    public class Slash : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        private void OnTriggerEnter2D(Collider2D col)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.OnTakeDamagehandler(weaponData.AbilityDamage);
            }
        }
    }
}