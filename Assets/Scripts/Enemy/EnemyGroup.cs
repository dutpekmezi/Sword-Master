using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace dutpekmezi
{
    [System.Serializable]
    public class EnemyGroup
    {
        public EnemyBase leader;
        public List<EnemyBase> followers = new List<EnemyBase>();
        public List<EnemyBase> allMembers = new List<EnemyBase>();

        public void UpdateGroup()
        {
            if (leader == null)
            {
                if (followers.Count > 0)
                {
                    leader = EnemySystem.Instance.FindFastestEnemy(this);
                    if (leader != null)
                    {
                        leader.SetAsLeader();

                        EnemyNavHelper.EnsureAgentOnNavMesh(leader);
                    }
                        
                    else
                        return;
                }
                else
                {
                    return;
                }
            }

            if (leader.Agent == null || !leader.Agent.isOnNavMesh)
                return;

            foreach (var follower in followers)
            {
                if (follower == null || follower.IsDead)
                    continue;

                Vector2 leaderPos = leader.transform.position;
                Vector2 followerPos = follower.transform.position;

                float dist = Vector2.Distance(followerPos, leaderPos);
                if (dist < WaveManager.Instance.StopDistance)
                    continue;

                Vector2 targetPos = leaderPos + (Random.insideUnitCircle * WaveManager.Instance.OrbitRadius);
                Vector2 dir = (targetPos - followerPos).normalized;
                Vector2 newPos = followerPos + dir * follower.EnemyData.MoveSpeed * Time.deltaTime;

                follower.transform.position = new Vector3(newPos.x, newPos.y, follower.transform.position.z);
            }
        }

        public void SetSubscribes(List<EnemyBase> enemies)
        {
            foreach (EnemyBase enemy in enemies)
            {
                enemy.OnDeath += RemoveEnemy;
            }
        }

        private void RemoveEnemy(EnemyBase enemy)
        {
            followers.Remove(enemy);
            allMembers.Remove(enemy);

            if (enemy.IsLeader)
            {
                leader = null;
            }
        }
    }
}
