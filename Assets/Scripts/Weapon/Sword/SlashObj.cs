using UnityEngine;
using DG.Tweening;
using Utils.Signal;

namespace dutpekmezi
{
    public class SlashObj : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        private void OnTriggerEnter2D(Collider2D col)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                SignalBus.Get<EnemyBase.OnTakeDamage>().Invoke(enemy, weaponData.AbilityDamage);
            }
        }
    }
}