using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class Move_Module : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f; // �������� �������� ��������
    [SerializeField] private float acceleration = 10f; // ���������
    [SerializeField] private float maxCameraDistance = 5f;
    [SerializeField] private float maxCharacterSpeed = 10f; // ��������, ��� ������� ����������� ����������
    [SerializeField] private float airControl = 0.5f; // ������� ���������� � �������
    [SerializeField] private GameObject Damager;
    [SerializeField] private Humanoid _humanoid;
    [SerializeField] private LayerMask AttackLayer;
    [SerializeField] private float AutoAttackRaduis = 1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -9.81f; // ����������� �������� ��� ����������
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Animator _Animator;
    [SerializeField] private EnemyTargetComponent _enemyTargetComponent;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 playerVelocity = Vector3.zero; // ������� ��������
    [SerializeField] private Vector3 cameraOffset = new(0f,.5f,0f); // ������� ��������
    [SerializeField] private TMP_Text YouDiedText;
    [SerializeField] private Image BlackGround;
    public CharacterController _characterController;
    public Transform _mainCamera;
    public PlayerController _movement;
    private readonly List<(float speedMultiplier, float durationInMilliseconds)> speedEffects = new();
    private float TotalSpeedMulti = 1f;
    public Vector2 velocity = Vector2.zero;
    public float magnitude = 0.25f;
    public bool Jumping = false; // ���� ������
    public bool OnJumping = false; // ���� ��������� ������
    public bool isGrounded; // �������� �� ���������� �� �����
    public bool CameraShiftLock = false;
    public float minSmoothing = 0.125f; // ����������� �������� �����������
    public float maxSmoothing = 0.125f;  // ������������ �������� �����������
    public float attackTime = 0.5f;           // ����� ����� (��� ��������)
    public float cooldownTime = 1f;          // ����� ������ ����� �����

    private Vector3 lastCharacterPosition; // ��������� ������� ���������
    private Vector3 currentVelocity; // �������� ��� SmoothDamp
    private bool BasicAttackCooldown =false;
    private bool _died = false;

    private void Start()
    {
        _humanoid.OnTakeDamaged += TakeDamaged;
        _humanoid.OnDied += onDied;
    }
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
        _enemyTargetComponent.Died = true;
        YouDiedText.enabled = true;
        BlackGround.enabled = true;
        _died = true;
        _Animator.SetBool("Died",true);
        _characterController.enabled = false;
    }
    private void TakeDamaged()
    {
        speedEffects.Add((0.1f, 0.25f));
        _Animator.SetTrigger("TakeDamage");
    }

    private void FixedUpdate()
    {
        if (!_mainCamera)
            return;
        HandleMovement();
        _movement.RotateHandle();
        HandleCameraFollow();
    }
    private void Update()
    {
        if (!_mainCamera) return;
        TotalSpeedMulti = 1f;
        for (int i = 0; i < speedEffects.Count; i++)
        {
            // ��������� ������������ ������� ������� �� ����� ����������
            var effect = speedEffects[i];
            effect.durationInMilliseconds -= Time.deltaTime; // ������������ � ������������
            TotalSpeedMulti *= effect.speedMultiplier;  // �������� ������ ���������
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
    private void HandleMovement()
    {
        if(_died)return;
        // ��������, ��������� �� ����� �� �����
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
       
        // ������� ������� ��������
        Vector3 targetSpeed = (_movement.nextPosition.magnitude >= 0.01f ? 1f : 0f) * TotalSpeedMulti * walkSpeed * _movement.nextPosition;

        // ��������
        _Animator.SetFloat("Speed", targetSpeed.magnitude);
        _Animator.SetBool("IsGround", isGrounded);

        if (isGrounded)
        {
            // �� ����� ���������� ������ � ����������
            playerVelocity = new Vector3(
                Mathf.MoveTowards(playerVelocity.x, targetSpeed.x, acceleration * Time.fixedDeltaTime),
                playerVelocity.y, // ������������ �������� �� ���������� �� ���� �����
                Mathf.MoveTowards(playerVelocity.z, targetSpeed.z, acceleration * Time.fixedDeltaTime)
            );

            // ���������� ����������
            if (!OnJumping && !Jumping)
            {
                playerVelocity.y = -0.5f; // ���������� ������ �� �����
            }
        }
        else
        {
            // � ������� ���������� ������� � �����������
            playerVelocity = new Vector3(
                Mathf.Lerp(playerVelocity.x, targetSpeed.x, airControl * Time.fixedDeltaTime),
                Mathf.Clamp(playerVelocity.y + gravity * Time.fixedDeltaTime, gravity, 100f), // ���������� ����������
                Mathf.Lerp(playerVelocity.z, targetSpeed.z, airControl * Time.fixedDeltaTime)
            );
        }

        // ������
        if (Jumping && !OnJumping && isGrounded)
        {
            _Animator.SetTrigger("IsJump");
            OnJumping = true;
            playerVelocity.y = 0f;
            StartCoroutine(JumpCoroutine(new WaitForSeconds(0.2f), jumpForce));
        }

        // ����������� ������
        _characterController.Move(playerVelocity * Time.fixedDeltaTime);

        // ������� ���������
        HandleRotation();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(Vector3.zero), AutoAttackRaduis);
    }
    public void BasicAttack()
    {
        if(!isGrounded||BasicAttackCooldown || _died)
            return;
        StartCoroutine(BasicAttackCourutine(attackTime,new(cooldownTime)));
    }
    private IEnumerator BasicAttackCourutine(float attackTime, WaitForSeconds CooldownTime)
    {
        Transform Target = null;
        Collider[] hitColliders = Physics.OverlapSphere(_characterController.transform.position, AutoAttackRaduis, AttackLayer);
        AddSpeedEffect(0f, attackTime);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.gameObject.TryGetComponent(out Humanoid humanoid);
            if (humanoid != null)
            {
                Target = humanoid.transform;
            }
        }
        if(Target!=null)
        {
            Vector3 targetDirection = Target.position - _characterController.transform.position;
            targetDirection.y = 0; // ��� ����� �������� �� ���������� �����/����, � ������ �� �����������
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _characterController.transform.rotation = Quaternion.Slerp(
            _characterController.transform.rotation,
            targetRotation,
            10f
             );
            Debug.Log("Look To1");
        }
        BasicAttackCooldown = true;
        _Animator.SetTrigger("Slash");
        DelltaActivator.EnableForTime(Damager, 0.1f);
        yield return new WaitForSeconds(0.01f);
        if (Target != null)
        {
            Vector3 targetDirection = Target.position - _characterController.transform.position;
            targetDirection.y = 0; // ��� ����� �������� �� ���������� �����/����, � ������ �� �����������
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _characterController.transform.rotation = Quaternion.Slerp(
            _characterController.transform.rotation,
            targetRotation,
            10f
             );
            Debug.Log("Look To2");
        }
        yield return new WaitForSeconds(attackTime);


        yield return CooldownTime;
        BasicAttackCooldown = false;
        yield return null;
    }
    private void HandleRotation()
    {
        if (playerVelocity.magnitude > 0.1f)
        {

                Vector3 movementDirection = new Vector3(playerVelocity.x, 0, playerVelocity.z).normalized;
                if (movementDirection.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                    _characterController.transform.rotation = Quaternion.Slerp(
                        _characterController.transform.rotation,
                        targetRotation,
                        Time.fixedDeltaTime * 16f
                    );
                }
            
        }
    }
    public void AddSpeedEffect(float speedMultiplier, float durationInMilliseconds)
    {
        speedEffects.Add((speedMultiplier, durationInMilliseconds));
    }
    private void HandleCameraFollow()
    {
        Vector3 characterPosition = _characterController.transform.position + cameraOffset;

        // ������������ ������� �������� ���������
        Vector3 characterVelocity = (characterPosition - lastCharacterPosition) / Time.fixedDeltaTime;
        lastCharacterPosition = characterPosition;

        // ���� �������� ������� ������ �� ������, ������ ������������ �����
        float distanceToCharacter = Vector3.Distance(_mainCamera.transform.position, characterPosition);
        if (distanceToCharacter > maxCameraDistance)
        {
            _mainCamera.transform.position = characterPosition; // ���������� �����������
            return;
        }

        // ������������ ����������� � ����������� �� �������� ���������
        float dynamicSmoothing = Mathf.Lerp(
            minSmoothing,
            maxSmoothing,
            Mathf.Clamp01(characterVelocity.magnitude / maxCharacterSpeed)
        );

        // ������� ���������� ������
        _mainCamera.transform.position = Vector3.SmoothDamp(
            _mainCamera.transform.position,
            characterPosition,
            ref currentVelocity,
            dynamicSmoothing
        );
    }

    private IEnumerator JumpCoroutine(WaitForSeconds interval, float force)
    {
        playerVelocity.y += force; // ��������� ���� ������
        yield return new WaitForSeconds(0.05f);
        while (!isGrounded)
        {
            yield return null;
        }
        yield return interval;
        OnJumping = false;
    }
}
