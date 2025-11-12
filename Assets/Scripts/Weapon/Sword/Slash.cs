using UnityEngine;
using DG.Tweening;

namespace dutpekmezi
{
    public class Slash : MonoBehaviour
    {
        [SerializeField] private WeaponData weapon;
        private void OnTriggerEnter2D(Collider2D col)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.TakeDamage(weapon.AbilityDamage);
            }
        }
    }
}