using UnityEngine;


namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Game/Scriptable Objects/Wave/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Pre Chaos Wave Time Settings")]
        public float preChaosDuration;

        [Header("Wave Settings")]
        public EnemyWaveConfig enemyWaveConfig;

        [Header("Statue Settings")]
        public StatueSpawnConfig statueSpawnConfig;

        [Header("Chest Settings")]
        public ChestSpawnConfig chestSpawnConfig;
    }
}
