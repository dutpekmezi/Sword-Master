using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "AbilityDatas", menuName = "Game/Scriptable Objects/Ability/AbilityDatas")]
    public class AbilityDatas : ScriptableObject
    {
        public List<AbilityBase> Abilities;
    }
}