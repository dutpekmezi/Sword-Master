using UnityEngine;

namespace dutpekmezi
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private int enemyCount = 10;

        [SerializeField] private float radius = 5;
        [SerializeField] private float deflection = 1;

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
                GenerateWave();
            }
        }

        private void GenerateWave()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                var randomPos = GenerateRandomPos();

                var instance = EnemySystem.Instance.CreateRandomEnemy();
                instance.transform.position = randomPos;
            }
        }

        private Vector2 GenerateRandomPos()
        {
            Vector2 center = CharacterSystem.Instance.GetCharacterTransform().position;

            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            float distance = radius + Random.Range(-deflection, deflection);

            float x = center.x + Mathf.Cos(angle) * distance;
            float y = center.y + Mathf.Sin(angle) * distance;

            return new Vector2(x, y);
        }

    }
}