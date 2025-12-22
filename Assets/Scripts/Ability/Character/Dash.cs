using DG.Tweening;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "Dash", menuName = "Game/Scriptable Objects/Ability/Character/Dash")]
    public class Dash : AbilityBase<CharacterBase>
    {
        [Header("Dash Settings")]
        [SerializeField] private float pushForce = 5f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private Ease dashEase = Ease.OutSine;

        protected override void ExecuteAbility(CharacterBase character)
        {
            Vector2 dashDirection = character.GetMoveDirection();

            if (dashDirection.sqrMagnitude < 0.01f) return;

            Vector3 targetPosition = character.transform.position + (Vector3)dashDirection.normalized * pushForce;

            character.transform.DOMove(targetPosition, dashDuration)
                .SetEase(dashEase);
        }

        protected override bool CanUse(CharacterBase character)
        {
            return base.CanUse(character);
        }
    }
}