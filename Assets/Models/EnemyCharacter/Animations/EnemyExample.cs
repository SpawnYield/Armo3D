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
    [SerializeField] private GameObject _HpCanvas;
    [SerializeField] private Humanoid _humanoid;
    private float EnemySpeedMultiplayer = 1f;
    private bool Attacked = false;
    private SynchronizationContext unityContext;
    private EnemyTargetComponent _targetComponent; // Кэшируем ссылку на EnemyTargetComponent
    private Transform Target;
    public bool Stuned { get;private set;}
    private void Start()
    {
        _HpCanvas.SetActive(true);
        characterController.enabled = true;
        unityContext = SynchronizationContext.Current;
        agent.enabled = true;
        _humanoid.OnTakeDamaged += TakeDamaged;
        _humanoid.OnDied += onDied;
        AgentPositionSync(200);
        EnemyAttack(200);
    }
    [SerializeField] private readonly List<(float speedMultiplier, float durationInMilliseconds)> speedEffects = new();
    [SerializeField] private readonly List<float> stuns = new();

    private void OnDestroy()
    {
        if (_humanoid != null)
        {
            _humanoid.OnTakeDamaged -= TakeDamaged;
            _humanoid.OnDied -= onDied;
        }
    }
    private void onDied()
    {
        _Animator.SetFloat("speed", 0f);
        _Animator.SetBool("Died",true);
        _HpCanvas.SetActive(false);
        characterController.enabled = false;
    }
    private void TakeDamaged()
    {
        speedEffects.Add((0.1f, 0.25f));
        _Animator.SetTrigger("TakeDamage");
    }
    private void SpeedUpdate()
    {
        EnemySpeedMultiplayer = 1f;
        if (speedEffects.Count != 0)
        {
            for (int i = 0; i < speedEffects.Count; i++)
            {
                // Уменьшаем длительность каждого эффекта на время обновления
                var effect = speedEffects[i];
                effect.durationInMilliseconds -=1000f*Time.fixedDeltaTime; // Конвертируем в миллисекунды
                EnemySpeedMultiplayer *= effect.speedMultiplier;  // Умножаем каждый множитель
                                                       // Если длительность эффекта закончена, удаляем его из списка
                if (effect.durationInMilliseconds <= 0)
                {
                    speedEffects.RemoveAt(i);
                    i--;  // Уменьшаем индекс, чтобы не пропустить следующий элемент
                }
                else
                {
                    // Обновляем эффект в списке
                    speedEffects[i] = effect;
                }
            }
        }
        if(stuns.Count != 0)
        {
            for (int i = 0; i < stuns.Count; i++)
            {
                // Уменьшаем длительность каждого эффекта на время обновления
                var effect = stuns[i];
                effect -= 1000f * Time.fixedDeltaTime; // Конвертируем в миллисекунды
                Debug.Log(effect);
                
                if (effect <= 0)
                {
                    stuns.RemoveAt(i);
                    i--;  // Уменьшаем индекс, чтобы не пропустить следующий элемент
                }
                else
                {
                    // Обновляем эффект в списке
                    stuns[i] = effect;
                }

            }
            Stuned =true;
        }
        else
        {
            Stuned =false;
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
                    Target = EntityManager.GetTarget(Distance, characterController != null? characterController.transform.position:Vector3.zero, SortType.None);
                    if(Target != null)
                        _targetComponent = Target.GetComponent<EnemyTargetComponent>();


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
                
                    if (_humanoid.Died)
                    {
                        return;
                    }
                    SpeedUpdate();
                    _Animator.SetBool("isGround", Physics.CheckSphere(groundCheckTransform.position, GroundCheckRadius, GroundCheckLayer));
                    _Animator.SetFloat("speed", agent.velocity.magnitude*agent.speed);

                    if (_targetComponent != null && _targetComponent.Died)
                        Target = null;
                    if(Target!= null)
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
            DelltaActivator.EnableForTime(Damager, 0.1f);
            speedEffects.Add((.01f, 100f));
            yield return Cooldown;
            Attacked =false;
            yield return null;
        };

        while (true)
        {
            if (this != null && Target != null && Vector3.Distance(Target.position,transform.position)< AttackDistance)
            {
                if(Stuned)
                    return;
                if (_humanoid != null)
                    if (_humanoid.Died)
                        return;
                if (!Attacked)
                {
                    Vector3 targetDirection = Target.position - characterController.transform.position;
                    targetDirection.y = 0; // Это чтобы персонаж не наклонялся вверх/вниз, а только по горизонтали
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    StartCoroutine(Attack(new(AttackCooldown), targetRotation));
                }

            }
            await Task.Delay(intervalMs);
        }
    }

}
