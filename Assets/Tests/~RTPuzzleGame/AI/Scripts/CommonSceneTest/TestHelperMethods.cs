using UnityEngine;
using UnityEngine.AI;

namespace Tests
{
    public class TestHelperMethods
    {
        public static void SetAgentDestinationPositionReached(NavMeshAgent agent)
        {
            agent.velocity = Vector3.zero;
            agent.nextPosition = agent.destination;
            agent.transform.position = agent.destination;
        }

        /*
        public static void SetAgentDestinationPositionReached( AIObjectType ai, Vector3 worldPosition)
        {
            var agent = ai.GetAgent();
            agent.Warp(worldPosition);
            agent.SetDestination(worldPosition);
            agent.velocity = Vector3.zero;
            agent.transform.position = agent.destination;
            agent.ResetPath();
            ai.OnDestinationReached();
        }
        */

        public static void SetAgentPosition(NavMeshAgent agent, Vector3 worldPosition)
        {
            agent.velocity = Vector3.zero;
            agent.nextPosition = worldPosition;
            agent.transform.position = worldPosition;
        }
    }
}