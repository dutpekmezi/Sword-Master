using UnityEngine;


namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Game/Scriptable Objects/Wave/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Wave Entites Settings")]
        public Transform waveEntitiesHolder;
        

        [Header("Wave Time Settings")]
        public float preChaosDuration;
        public float preChaosWaveSpawnRate;
        public float preChaosGroupSpawnRate;

        [Header("Enemy Wave Settings")]
        public int enemiesPerWave;
        public float waveSpawnRadius;
        public float waveSpawnDeflection;

        [Header("Enemy Group Wave Settings")]
        public int enemiesPerGroup;
        public float groupSpawnRadius;
        public float groupSpawnDeflection;
        public float enemyGroupRadius;
        public float enemyGroupDeflection;

        [Header("Statues")]
        public int statuesPerWave;
        public float statueSpawnRadius;
        public float statueSpawnDeflection;
    }
}