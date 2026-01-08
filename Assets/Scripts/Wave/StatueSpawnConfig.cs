using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "StatueSpawnConfig", menuName = "Game/Scriptable Objects/Wave/StatueSpawnConfig")]
    public class StatueSpawnConfig : ScriptableObject
    {
        [Header("Statue Settings")]
        public int statuesPerWave;
        public float statueSpawnRadius;
        public float statueSpawnDeflection;
        public float statueMinimumDistance;
        public float maxStatue;
        public float maxStatStatue;
        public float maxWeaponStatue;
    }
}
