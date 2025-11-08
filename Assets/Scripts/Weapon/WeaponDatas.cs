using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "WeaponDatas", menuName = "Game/Scriptable Objects/Weapon/WeaponDatas")]
    public class WeaponDatas : ScriptableObject
    {
        public List<WeaponData> weapons;
    }
}