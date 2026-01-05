using UnityEngine;


namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Game/Scriptable Objects/Wave/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Pre Chaos Wave Time Settings")]
        public float preChaosDuration;
        public float preChaosWaveSpawnRate;
        public float preChaosGroupSpawnRate;

        [Header("Enemy Settings")]
        public float enemyStatScaleFactor;

        [Header("Pre Chaos Enemy Wave Settings")]
        public int enemiesPerWave;
        public float waveSpawnRadius;
        public float waveSpawnDeflection;

        [Header("Chaos Enemy Wave Settings")]
        public int chaosEnemiesPerWave;
        public float chaosWaveSpawnRate;

        [Header("Enemy Group Wave Settings")]
        public int enemiesPerGroup;
        public float groupSpawnRadius;
        public float groupSpawnDeflection;
        public float enemyGroupRadius;
        public float enemyGroupDeflection;

        [Header("Chaos Enemy Group Settings")]
        public int chaosEnemiesPerGroup;
        public float chaosGroupSpawnRate;

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
