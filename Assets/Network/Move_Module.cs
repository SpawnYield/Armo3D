using System.Collections;
using UnityEngine;

public class Move_Module : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f; // Основная скорость движения
    [SerializeField] private float acceleration = 10f; // Ускорение
    [SerializeField] private float maxCameraDistance = 5f;
    public float minSmoothing = 0.125f; // Минимальное значение сглаживания
    public float maxSmoothing = 0.125f;  // Максимальное значение сглаживания
    [SerializeField] private float maxCharacterSpeed = 10f; // Скорость, при которой сглаживание минимально

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -9.81f; // Стандартное значение для гравитации
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Animator _Animator;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 playerVelocity = Vector3.zero; // Текущая скорость
    [SerializeField] private Vector3 cameraOffset = new(0f,.5f,0f); // Текущая скорость

    public CharacterController _characterController;
    public Transform _mainCamera;
    public PlayerController _movement;

    public Vector2 velocity = Vector2.zero;
    public float magnitude = 0.25f;
    public bool Jumping = false; // Флаг прыжка
    public bool OnJumping = false; // Флаг активного прыжка
    public bool isGrounded; // Проверка на нахождение на земле
    public bool CameraShiftLock = false;

    private Vector3 lastCharacterPosition; // Последняя позиция персонажа
    private Vector3 currentVelocity; // Скорость для SmoothDamp

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
        // Проверка, находится ли игрок на земле
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Рассчет целевой скорости
        Vector3 targetSpeed = (_movement.nextPosition.magnitude >= 0.01f ? 1f : 0f) * walkSpeed * _movement.nextPosition;
        _Animator.SetFloat("Speed", targetSpeed.magnitude);
        _Animator.SetBool("IsGround", isGrounded);

        // Плавное изменение текущей скорости
        playerVelocity = new Vector3(
            Mathf.MoveTowards(playerVelocity.x, targetSpeed.x, acceleration * Time.fixedDeltaTime),
            Mathf.Clamp(playerVelocity.y + gravity * Time.fixedDeltaTime, gravity, 100f),
            Mathf.MoveTowards(playerVelocity.z, targetSpeed.z, acceleration * Time.fixedDeltaTime)
        );

        // Применение гравитации
        if (isGrounded && !OnJumping && !Jumping)
        {
            playerVelocity.y = -0.5f; // Обнуляем вертикальную скорость при касании земли
        }

        // Прыжок
        if (Jumping && !OnJumping && isGrounded)
        {
            _Animator.SetTrigger("IsJump");
            OnJumping = true;
            playerVelocity.y = 0f;
            StartCoroutine(JumpCoroutine(new WaitForSeconds(0.2f), jumpForce));
        }

        // Перемещение игрока с помощью CharacterController
        _characterController.Move(playerVelocity * Time.fixedDeltaTime);

        // Поворот персонажа
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

        // Рассчитываем текущую скорость персонажа
        Vector3 characterVelocity = (characterPosition - lastCharacterPosition) / Time.fixedDeltaTime;
        lastCharacterPosition = characterPosition;

        // Если персонаж слишком далеко от камеры, камера перемещается сразу
        float distanceToCharacter = Vector3.Distance(_mainCamera.transform.position, characterPosition);
        if (distanceToCharacter > maxCameraDistance)
        {
            _mainCamera.transform.position = characterPosition; // Мгновенное перемещение
            return;
        }

        // Динамическое сглаживание в зависимости от скорости персонажа
        float dynamicSmoothing = Mathf.Lerp(
            minSmoothing,
            maxSmoothing,
            Mathf.Clamp01(characterVelocity.magnitude / maxCharacterSpeed)
        );

        // Плавное следование камеры
        _mainCamera.transform.position = Vector3.SmoothDamp(
            _mainCamera.transform.position,
            characterPosition,
            ref currentVelocity,
            dynamicSmoothing
        );
    }

    private IEnumerator JumpCoroutine(WaitForSeconds interval, float force)
    {
        playerVelocity.y += force; // Добавляем силу прыжка
        yield return new WaitForSeconds(0.05f);
        while (!isGrounded)
        {
            yield return null;
        }
        yield return interval;
        OnJumping = false;
    }
}
