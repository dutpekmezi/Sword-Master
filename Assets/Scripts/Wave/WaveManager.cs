using UnityEngine;

namespace dutpekmezi
{
    public class WaveManager : MonoBehaviour
    {
        [Header("ENemy Group Settings")]
        [SerializeField] private int enemiesPerGroup = 10;
        [SerializeField] private float spawnRadius = 5;
        [SerializeField] private float enemyGroupRadius = 5;
        [SerializeField] private float deflection = 1;
        [SerializeField] private float stopDistance = 0.8f;
        [SerializeField] private float orbitRadius = 1.5f;

        public float SpawRadius => spawnRadius;
        public float EnemyGroupRadius => enemyGroupRadius;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EnemySystem.Instance.CreateEnemyGroup(enemiesPerGroup);
            }
        }

        private void GenerateWave()
        {
            for (int i = 0; i < enemiesPerGroup; i++)
            {
                var randomPos = GenerateRandomPos(spawnRadius, CharacterSystem.Instance.GetCurrentCharacterTransform().position);

                var instance = EnemySystem.Instance.CreateRandomEnemy(randomPos);
                instance.transform.position = randomPos;
            }
        }

        public Vector2 GenerateRandomPos(float _radius, Vector2 _center)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            float distance = _radius + Random.Range(-deflection, deflection);

            float x = _center.x + Mathf.Cos(angle) * distance;
            float y = _center.y + Mathf.Sin(angle) * distance;

            return new Vector2(x, y);
        }

    }
}