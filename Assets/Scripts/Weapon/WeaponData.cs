using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Scriptable Objects/Weapon/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string Id; // Id
        public string Name; // The Name of the weapon
        public string Description; // Detail sentence about the weapon

        public Sprite Icon;

        public float OrbitSpeed; // Spin speed around the character
        public float SelfOrbitSpeed; // Self spin speed

        public int AttackDamage; // Damage value when collide while spin
        public int AbilityDamage; // Damage value of ability

        public WeaponBase Prefab;

        public WeaponType WeaponType;
    }

    public enum WeaponType
    {
        Sword,
        Shield,
        Gun
    }
}