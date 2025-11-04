using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Scriptable Objects/Character/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        public string Id;
        public string Name;

        public int MaxHealth;

        public WeaponType WeaponType;

        public CharacterBase Prefab;
    }
}