using UnityEngine;
using UnityEngine.AI;

public class WaitForNavAgentDestinationReached : CustomYieldInstruction
{

    private NavMeshAgent agent;

    public WaitForNavAgentDestinationReached(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override bool keepWaiting
    {
        get
        {
            return !(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f));
        }
    }
}
