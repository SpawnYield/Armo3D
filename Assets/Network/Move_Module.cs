using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Move_Module : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f; // Основная скорость движения
    [SerializeField] private float acceleration = 10f; // Ускорение
    [SerializeField] private float maxCameraDistance = 5f;
    [SerializeField] private float maxCharacterSpeed = 10f; // Скорость, при которой сглаживание минимально
    [SerializeField] private float airControl = 0.5f; // Степень управления в воздухе

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
    private List<(float speedMultiplier, float durationInMilliseconds)> speedEffects = new();
    private float TotalSpeedMulti = 1f;
    public Vector2 velocity = Vector2.zero;
    public float magnitude = 0.25f;
    public bool Jumping = false; // Флаг прыжка
    public bool OnJumping = false; // Флаг активного прыжка
    public bool isGrounded; // Проверка на нахождение на земле
    public bool CameraShiftLock = false;
    public float minSmoothing = 0.125f; // Минимальное значение сглаживания
    public float maxSmoothing = 0.125f;  // Максимальное значение сглаживания
    public float attackTime = 0.5f;           // Время атаки (для анимации)
    public float cooldownTime = 1f;          // Время отката после атаки

    private Vector3 lastCharacterPosition; // Последняя позиция персонажа
    private Vector3 currentVelocity; // Скорость для SmoothDamp
    private bool BasicAttackCooldown =false;
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
            // Уменьшаем длительность каждого эффекта на время обновления
            var effect = speedEffects[i];
            effect.durationInMilliseconds -= Time.fixedDeltaTime; // Конвертируем в миллисекунды
            TotalSpeedMulti *= effect.speedMultiplier;  // Умножаем каждый множитель
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
    private void HandleMovement()
    {
        // Проверка, находится ли игрок на земле
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
       
        // Рассчет целевой скорости
        Vector3 targetSpeed = (_movement.nextPosition.magnitude >= 0.01f ? 1f : 0f) * TotalSpeedMulti * walkSpeed * _movement.nextPosition;

        // Анимация
        _Animator.SetFloat("Speed", targetSpeed.magnitude);
        _Animator.SetBool("IsGround", isGrounded);

        if (isGrounded)
        {
            // На земле управление резкое и отзывчивое
            playerVelocity = new Vector3(
                Mathf.MoveTowards(playerVelocity.x, targetSpeed.x, acceleration * Time.fixedDeltaTime),
                playerVelocity.y, // Вертикальная скорость не изменяется на этом этапе
                Mathf.MoveTowards(playerVelocity.z, targetSpeed.z, acceleration * Time.fixedDeltaTime)
            );

            // Применение гравитации
            if (!OnJumping && !Jumping)
            {
                playerVelocity.y = -0.5f; // Удерживаем игрока на земле
            }
        }
        else
        {
            // В воздухе управление плавное и инерционное
            playerVelocity = new Vector3(
                Mathf.Lerp(playerVelocity.x, targetSpeed.x, airControl * Time.fixedDeltaTime),
                Mathf.Clamp(playerVelocity.y + gravity * Time.fixedDeltaTime, gravity, 100f), // Применение гравитации
                Mathf.Lerp(playerVelocity.z, targetSpeed.z, airControl * Time.fixedDeltaTime)
            );
        }

        // Прыжок
        if (Jumping && !OnJumping && isGrounded)
        {
            _Animator.SetTrigger("IsJump");
            OnJumping = true;
            playerVelocity.y = 0f;
            StartCoroutine(JumpCoroutine(new WaitForSeconds(0.2f), jumpForce));
        }

        // Перемещение игрока
        _characterController.Move(playerVelocity * Time.fixedDeltaTime);

        // Поворот персонажа
        HandleRotation();
    }

    public void BasicAttack()
    {
        if(!isGrounded||BasicAttackCooldown )
            return;
        StartCoroutine(BasicAttackCourutine(attackTime,new(cooldownTime)));
    }
    private IEnumerator BasicAttackCourutine(float attackTime, WaitForSeconds CooldownTime)
    {
        BasicAttackCooldown = true;
        _Animator.SetTrigger("Slash");
        AddSpeedEffect(.01f,attackTime);
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
