using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "ChestSpawnConfig", menuName = "Game/Scriptable Objects/Wave/ChestSpawnConfig")]
    public class ChestSpawnConfig : ScriptableObject
    {
        [Header("Chest Settings")]
        public int chestsPerWave;
        public float chestSpawnRadius;
        public float chestSpawnDeflection;
        public float chestMinimumDistance;
        public int maxChest;
    }
}
