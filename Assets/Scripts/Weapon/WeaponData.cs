using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Scriptable Objects/Weapon/WeaponData")]
    public class WeaponData : EntityData
    {
        public Sprite Icon;

        public float OrbitRadius; // Distance between weapon and character
        public float OrbitSpeed; // Spin speed around the character
        public float SelfOrbitSpeed; // Self spin speed

        public int AttackDamage; // Damage value when collide while spin
        public int AbilityDamage; // Damage value of ability

        public WeaponType WeaponType;

        public AbilityBase AbilityData;

        public new WeaponBase Prefab => (WeaponBase)base.Prefab;
    }

    public enum WeaponType
    {
        Sword,
        Shield,
        Gun
    }
}
