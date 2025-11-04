using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "EnemyDatas", menuName = "Game/Scriptable Objects/Enemy/EnemyDatas")]
    public class EnemyDatas : ScriptableObject
    {
        public List<EnemyData> Enemies;
    }
}