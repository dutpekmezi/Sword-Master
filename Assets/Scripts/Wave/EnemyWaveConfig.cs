using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "EnemyWaveConfig", menuName = "Game/Scriptable Objects/Wave/EnemyWaveConfig")]
    public class EnemyWaveConfig : ScriptableObject
    {
        [Header("Pre Chaos Wave Time Settings")]
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
    }
}
