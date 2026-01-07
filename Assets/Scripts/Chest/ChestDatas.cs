using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "ChestDatas", menuName = "Game/Scriptable Objects/Chest/ChestDatas")]
    public class ChestDatas : ScriptableObject
    {
        public List<ChestData> Chests;
    }
}
