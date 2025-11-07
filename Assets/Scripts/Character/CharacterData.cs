using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Scriptable Objects/Character/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        public string Id;
        public string Name;

        public Sprite Icon;

        public int MaxHealth;
        public int MaxEnergy;

        public float MoveSpeed;

        public WeaponType WeaponType;

        public CharacterBase Prefab;
    }
}