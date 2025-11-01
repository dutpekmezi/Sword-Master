using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Scriptable Objects/Weapon/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string Id; // Id
        public string Name; // The Name of the weapon
        public string Description; // Detail sentence about the weapon

        public float OrbitSpeed; // Spin speed around the character
        public float SelfOrbitSpeed; // Self spin speed

        public float AttackDamage; // Damage value when collide while spin
        public float AbilityDamage; // Damage value of ability

        public GameObject Prefab;
    }
}