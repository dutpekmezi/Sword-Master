using UnityEngine;

namespace dutpekmezi
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Enemy Group Settings")]
        [SerializeField] private int enemiesPerGroup = 10;
        [SerializeField] private float groupSpawnRadius = 20;
        [SerializeField] private float groupSpawnDeflection = 5;
        [SerializeField] private float enemyGroupRadius = 5;
        [SerializeField] private float enemyGroupDeflection = 2;
        [SerializeField] private float stopDistance = 0.8f;
        [SerializeField] private float orbitRadius = 1.5f;

        [Header("Enemy Wawe Settings")]
        [SerializeField] private int enemiesPerWawe;
        [SerializeField] private float enenmyWaweRadius = 20;
        [SerializeField] private float enemyWaweDeflection = 1;

        public float GroupSpawRadius => groupSpawnRadius;
        public float GroupSpawnDeflection => groupSpawnDeflection;
        public float EnemyGroupRadius => enemyGroupRadius;
        public float EnemyGroupDeflection => enemyGroupDeflection;
        public float StopDistance => stopDistance;
        public float OrbitRadius => orbitRadius;
        public int EnemiesPerGroup => enemiesPerGroup;

        private static WaveManager instance;
        public static WaveManager Instance => instance;

        private void Start()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance);
            }

            instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                EnemySystem.Instance.CreateEnemyGroup(enemiesPerGroup);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateWave(enemiesPerGroup);
            }
        }

        private void GenerateWave(int enemyCount)
        {
            for (int i = 0; i < enemiesPerWawe; i++)
            {
                var randomPos = GenerateRandomPos(enenmyWaweRadius, enemyWaweDeflection, CharacterSystem.Instance.GetCurrentCharacterTransform().position);

                var instance = EnemySystem.Instance.CreateRandomEnemy(randomPos);
                instance.transform.position = randomPos;
            }
        }

        public Vector2 GenerateRandomPos(float _radius, float _deflection, Vector2 _center)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            float distance = _radius + Random.Range(-_deflection, _deflection);

            float x = _center.x + Mathf.Cos(angle) * distance;
            float y = _center.y + Mathf.Sin(angle) * distance;

            return new Vector2(x, y);
        }

    }
}