using System.Collections;
using UnityEngine;

public class Move_Module : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f; // �������� �������� ��������
    [SerializeField] private float acceleration = 10f; // ���������
    [SerializeField] private float maxCameraDistance = 5f;
    public float minSmoothing = 0.125f; // ����������� �������� �����������
    public float maxSmoothing = 0.125f;  // ������������ �������� �����������
    [SerializeField] private float maxCharacterSpeed = 10f; // ��������, ��� ������� ����������� ����������

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -9.81f; // ����������� �������� ��� ����������
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Animator _Animator;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 playerVelocity = Vector3.zero; // ������� ��������
    [SerializeField] private Vector3 cameraOffset = new(0f,.5f,0f); // ������� ��������

    public CharacterController _characterController;
    public Transform _mainCamera;
    public PlayerController _movement;

    public Vector2 velocity = Vector2.zero;
    public float magnitude = 0.25f;
    public bool Jumping = false; // ���� ������
    public bool OnJumping = false; // ���� ��������� ������
    public bool isGrounded; // �������� �� ���������� �� �����
    public bool CameraShiftLock = false;

    private Vector3 lastCharacterPosition; // ��������� ������� ���������
    private Vector3 currentVelocity; // �������� ��� SmoothDamp

    private void FixedUpdate()
    {
        if (!_mainCamera)
            return;

        HandleMovement();
        _movement.RotateHandle();
        HandleCameraFollow();
    }

    private void HandleMovement()
    {
        // ��������, ��������� �� ����� �� �����
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // ������� ������� ��������
        Vector3 targetSpeed = (_movement.nextPosition.magnitude >= 0.01f ? 1f : 0f) * walkSpeed * _movement.nextPosition;
        _Animator.SetFloat("Speed", targetSpeed.magnitude);
        _Animator.SetBool("IsGround", isGrounded);

        // ������� ��������� ������� ��������
        playerVelocity = new Vector3(
            Mathf.MoveTowards(playerVelocity.x, targetSpeed.x, acceleration * Time.fixedDeltaTime),
            Mathf.Clamp(playerVelocity.y + gravity * Time.fixedDeltaTime, gravity, 100f),
            Mathf.MoveTowards(playerVelocity.z, targetSpeed.z, acceleration * Time.fixedDeltaTime)
        );

        // ���������� ����������
        if (isGrounded && !OnJumping && !Jumping)
        {
            playerVelocity.y = -0.5f; // �������� ������������ �������� ��� ������� �����
        }

        // ������
        if (Jumping && !OnJumping && isGrounded)
        {
            _Animator.SetTrigger("IsJump");
            OnJumping = true;
            playerVelocity.y = 0f;
            StartCoroutine(JumpCoroutine(new WaitForSeconds(0.2f), jumpForce));
        }

        // ����������� ������ � ������� CharacterController
        _characterController.Move(playerVelocity * Time.fixedDeltaTime);

        // ������� ���������
        HandleRotation();
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
