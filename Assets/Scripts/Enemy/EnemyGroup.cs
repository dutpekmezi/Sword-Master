using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace dutpekmezi
{
    [System.Serializable]
    public class EnemyGroup
    {
        public List<EnemyBase> members = new List<EnemyBase>();

        public void SetSubscribes(List<EnemyBase> enemies)
        {
            foreach (EnemyBase enemy in enemies)
            {
                enemy.OnDeath += RemoveEnemy;
            }
        }

        private void RemoveEnemy(EnemyBase enemy)
        {
            members.Remove(enemy);
            members.Remove(enemy);
        }
    }
}
