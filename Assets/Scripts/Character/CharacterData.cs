using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Scriptable Objects/Character/CharacterData")]
    public class CharacterData : EntityData
    {
        public WeaponType WeaponType;
        public AbilityBase AbilityData;
    }
}