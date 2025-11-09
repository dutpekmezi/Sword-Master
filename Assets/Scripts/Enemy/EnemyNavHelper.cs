using UnityEngine;
using UnityEngine.AI;

namespace dutpekmezi
{
    public static class EnemyNavHelper
    {
        public static void EnsureAgentOnNavMesh(NavMeshAgent agent, Vector2 currentPosition)
        {
            if (agent == null) return;

            Vector2 fixedPos = new Vector2(currentPosition.x, currentPosition.y);

            if (!agent.enabled)
                agent.enabled = true;

            if (!agent.isOnNavMesh)
            {
                agent.enabled = false;

                agent.Warp(fixedPos);

                agent.enabled = true;

                agent.isStopped = false;
            }

            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        public static void EnsureAgentOnNavMesh(EnemyBase enemy)
        {
            if (enemy == null || enemy.Agent == null) return;

            EnsureAgentOnNavMesh(enemy.Agent, enemy.transform.position);
        }
    }
}
