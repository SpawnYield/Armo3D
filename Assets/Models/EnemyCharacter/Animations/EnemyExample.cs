using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyExample : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator _Animator;
    [SerializeField] private float Distance = 25f;
    [SerializeField] private float AttackDistance = 5f;
    [SerializeField] private float AttackCooldown = 1f;
    [SerializeField] private float GroundCheckRadius = 1f;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private LayerMask GroundCheckLayer;
    [SerializeField] private float EnemySpeed = 1f;
    [SerializeField] private GameObject Damager;

    private float EnemySpeedMultiplayer = 1f;
    private bool Attacked = false;
    private SynchronizationContext unityContext;
    private Transform Target;
    private void Start()
    {
        unityContext = SynchronizationContext.Current;
        agent.enabled = true;
        AgentPositionSync(200);
        EnemyAttack(200);
    }
    [SerializeField] private readonly List<(float speedMultiplier, float durationInMilliseconds)> speedEffects = new();

    private void SpeedUpdate()
    {
        EnemySpeedMultiplayer = 1f;
        if (speedEffects.Count != 0)
        {
            for (int i = 0; i < speedEffects.Count; i++)
            {
                // ��������� ������������ ������� ������� �� ����� ����������
                var effect = speedEffects[i];
                effect.durationInMilliseconds -=1000f*Time.fixedDeltaTime; // ������������ � ������������
                Debug.Log(effect);
                EnemySpeedMultiplayer *= effect.speedMultiplier;  // �������� ������ ���������
                                                       // ���� ������������ ������� ���������, ������� ��� �� ������
                if (effect.durationInMilliseconds <= 0)
                {
                    speedEffects.RemoveAt(i);
                    i--;  // ��������� ������, ����� �� ���������� ��������� �������
                }
                else
                {
                    // ��������� ������ � ������
                    speedEffects[i] = effect;
                }
            }
        }

        agent.speed = EnemySpeedMultiplayer*EnemySpeed;
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
            // ������������ � Unity ����� ��� ���������� CharacterController
            unityContext.Post(_ =>
            {
                if (this != null && agent != null)
                {
                    SpeedUpdate();
                    _Animator.SetBool("isGround", Physics.CheckSphere(groundCheckTransform.position, GroundCheckRadius, GroundCheckLayer));
                    _Animator.SetFloat("speed", agent.velocity.magnitude*agent.speed);
                    agent.SetDestination(Target.position);
                    characterController.Move(targetPosition);
                }
            }, null);
        }
    }
    private async void EnemyAttack(int intervalMs)
    {
        IEnumerator Attack(WaitForSeconds Cooldown,Quaternion targetRotation)
        {
            characterController.transform.rotation = Quaternion.Slerp(
                characterController.transform.rotation,
                targetRotation,
                100f
            );
            Attacked = true;
            _Animator.SetTrigger("Attack");
            _Animator.SetBool("Left",!_Animator.GetBool("Left"));
            DelltaActivator.EnableForTime(Damager, 0.15f);
            speedEffects.Add((.01f, 100f));
            yield return Cooldown;
            Attacked =false;
            yield return null;
        };

        while (true)
        {
            if (this != null && Target != null && Vector3.Distance(Target.position,transform.position)< AttackDistance)
            {
                if (!Attacked)
                {
                    Vector3 targetDirection = Target.position - characterController.transform.position;
                    targetDirection.y = 0; // ��� ����� �������� �� ���������� �����/����, � ������ �� �����������
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    StartCoroutine(Attack(new(AttackCooldown), targetRotation));
                }

            }
            await Task.Delay(intervalMs);
        }
    }

}
