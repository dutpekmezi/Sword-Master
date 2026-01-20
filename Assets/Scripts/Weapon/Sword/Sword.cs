using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Utils.Signal;

namespace dutpekmezi
{
    public class Sword : WeaponBase
    {
        [Header("Ability Settings")]
        [SerializeField] private SlashObj slash;
        [SerializeField] private float slashObjScaleX;
        [SerializeField] private float slashObjScaleY;
        [SerializeField] private float slashObjScaleDuration;
        [SerializeField] private float slashObjDeScaleDuration;


        protected override void OnTrigger(Entity entity)
        {
            SignalBus.Get<EnemyBase.OnTakeDamage>().Invoke(entity, weaponData.AttackDamage);
        }

        protected override void OnlevelUpHandler(int level)
        {
            var modifierValue = StatSystem.Instance.GetDefaultModifierValue(StatType.Scale, currentLevel);
            var modifier = StatSystem.Instance.CreateModifier(StatType.Scale, modifierValue);

            ApplyModifier(modifier); // bump size a bit when this blade levels
        }
    }
}
