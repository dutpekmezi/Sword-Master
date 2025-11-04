using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Scriptable Objects/Enemy/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string Id;
        public string Name;
        public string Description;

        public Sprite Sprite;

        public int MaxHealth;
        public int AttackDamage;

        public float MoveSpeed;

        public EnemyBase Prefab;
    }
}