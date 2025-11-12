using UnityEngine;
using DG.Tweening;

namespace dutpekmezi
{
    public class SilverFang : WeaponBase
    {
        [Header("Ability Settings")]
        [SerializeField] private Slash slash;
        [SerializeField] private float slashObjScaleX;
        [SerializeField] private float slashObjScaleY;
        [SerializeField] private float slashObjScaleDuration;
        [SerializeField] private float slashObjDeScaleDuration;

        private bool isSlashing = false;
        private Quaternion rotation;
        private Vector2 position;

        protected override void Ability()
        {
            if (Input.GetMouseButtonDown(0) && !isSlashing)
            {
                position = transform.position;
                rotation = transform.rotation;
                Slash();
            }
        }
        private void Slash()
        {
            isSlashing = false;

            var slashObj = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(
                slash, transform.position);

            slashObj.transform.position = position;
            slashObj.transform.localScale = Vector2.zero;
            slashObj.transform.rotation = rotation;
            slashObj.transform.DOScale(new Vector2(slashObjScaleX, slashObjScaleY), slashObjScaleDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    slashObj.transform.DOScale(Vector2.zero, slashObjDeScaleDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            DOTween.Kill(slashObj);
                            Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(slashObj.gameObject);
                            isSlashing = false;
                        });
                });
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.TakeDamage(weaponData.AttackDamage);
            }
        }
    }
}
