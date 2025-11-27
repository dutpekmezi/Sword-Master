using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils.Signal;

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
                SignalBus.Get<Entity.OnDeath>().Subscribe(RemoveEnemy);
            }
        }

        private void RemoveEnemy(Entity enemy)
        {
            members.Remove((EnemyBase)enemy);
            members.Remove((EnemyBase)enemy);
        }
    }
}
