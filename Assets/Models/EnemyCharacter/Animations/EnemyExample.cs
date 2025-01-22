using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyExample : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float Distance = 25f;

    private SynchronizationContext unityContext;
    private Transform Target;
    private void Start()
    {
        unityContext = SynchronizationContext.Current;
        agent.enabled = true;
        AgentPositionSync(100);
    }

    private async void AgentPositionSync(int intervalMs)
    {
        Vector3 targetPosition = Vector3.zero;
        while (true)
        {
            if (Target == null)
            {
                unityContext.Post(_ =>
                {
                    Target = EntityManager.GetTarget(Distance, characterController.transform.position, SortType.None);
                
                }, null);
                if(Target == null)
                {
                    await Task.Delay(intervalMs);
                    continue;
                }
            }

            if (this != null && agent != null)
                targetPosition = agent.nextPosition - transform.position;
            await Task.Delay(intervalMs);
            // Возвращаемся в Unity поток для обновления CharacterController
            unityContext.Post(_ =>
            {
                if (this != null && agent != null)
                {
                    agent.SetDestination(Target.position);
                    characterController.Move(targetPosition);
                }
            }, null);
        }
    }
}
