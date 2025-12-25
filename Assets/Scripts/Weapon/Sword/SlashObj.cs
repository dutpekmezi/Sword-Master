using UnityEngine;
using DG.Tweening;
using Utils.Signal;
using Dutpekmezi.Services.PoolService;

namespace dutpekmezi
{
    public class SlashObj : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;

        public void Init(SlashSettings settings)
        {
            transform.localScale = Vector2.zero;
            transform.rotation = settings.Rotation;

            transform.DOScale(new Vector2(settings.ScaleX, settings.ScaleY), settings.ScaleDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    transform.DOScale(Vector2.zero, settings.DeScaleDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(Cleanup);
                });
        }

        private void Cleanup()
        {
            DOTween.Kill(transform);
            ObjectPoolManager.DeSpawn(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out EnemyBase enemy))
            {
                SignalBus.Get<EnemyBase.OnTakeDamage>().Invoke(enemy, weaponData.AbilityDamage);
            }
        }
    }
}