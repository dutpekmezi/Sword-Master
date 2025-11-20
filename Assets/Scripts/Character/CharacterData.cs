using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Scriptable Objects/Character/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        public string Id;
        public string Name;

        public Sprite Icon;

        public List<Stat> BaseStats;

        public WeaponType WeaponType;

        public CharacterBase Prefab;
    }
}