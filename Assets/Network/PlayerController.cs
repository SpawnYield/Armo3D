
using Cinemachine;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private InputActionAsset _actionAsset;
    [SerializeField] private CinemachineVirtualCamera virtualLookCamera;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float MinZoom;
    [SerializeField] private float MaxZoom;
    [SerializeField] private GameObject CameraLockImage;

    private Cinemachine3rdPersonFollow cinemachine3rdPersonFollow;
    private InputActionMap _actionMap;
    private InputActionMap _actionMapUI;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction rotateAction;
    private InputAction scrollWhellAction;
    private InputAction cameraSwitchAction;

    public float2 move = new(0, 0);
    public float2 look = new(0, 0);
    public Vector3 nextPosition;
    public Quaternion nextRotation;

    public float rotationPower = .75f;
    public float rotationLerp = 0.5f;
    private bool OnRotate = false;

    public void Init()
    {
        SettingsManager.LoadSavedSensivity();
        rotationPower = SettingsManager.Sensivity;
        _actionMap = _actionAsset.FindActionMap("Player");
        _actionMapUI = _actionAsset.FindActionMap("UI");

        moveAction = _actionMap.FindAction("Move");
        moveAction.Enable();
        moveAction.started += OnMove;
        moveAction.performed += OnMove; // Обработчик, когда движение выполняется
        moveAction.canceled += OnMove; // Обработчик, когда движение отменяется

        lookAction = _actionMap.FindAction("Look");
        lookAction.Enable();
        lookAction.started += OnLook;
        lookAction.performed += OnLook;
        lookAction.canceled += OnLook; // Обработчик, когда движение отменяется

        jumpAction = _actionMap.FindAction("Jump");
        jumpAction.Enable();
        jumpAction.started += OnJumpStart;
        jumpAction.canceled += OnJumpEnd;

        rotateAction = _actionMap.FindAction("Rotate");
        rotateAction.Enable();
        rotateAction.started += RotateToTrue;
        rotateAction.canceled += RotateToFalse;

        cameraSwitchAction = _actionMap.FindAction("SwitchToCamera");
        cameraSwitchAction.Enable();
        cameraSwitchAction.started += SwitchCamera;

        scrollWhellAction = _actionMapUI.FindAction("ScrollWheel");
        scrollWhellAction.Enable();
        scrollWhellAction.performed += OnZoom;
        scrollWhellAction.started += OnZoom;
        scrollWhellAction.canceled += OnZoom;


        Game_Manager.instance._moveModule._mainCamera = Cameratarget;
        cinemachine3rdPersonFollow = virtualLookCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    public void Deinit()
    {
        if (moveAction != null)
        {
            moveAction.started -= OnMove;
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            moveAction.Disable();
        }

        if (lookAction != null)
        {
            lookAction.started -= OnLook;
            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;
            lookAction.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.started -= OnJumpStart;
            jumpAction.canceled -= OnJumpEnd;
            jumpAction.Disable();
        }

        if (rotateAction != null)
        {
            rotateAction.started -= RotateToTrue;
            rotateAction.canceled -= RotateToFalse;
            rotateAction.Disable();
        }

        if (cameraSwitchAction != null)
        {
            cameraSwitchAction.started -= SwitchCamera;
            cameraSwitchAction.Disable();
        }

        if (scrollWhellAction != null)
        {
            scrollWhellAction.started -= OnZoom;
            scrollWhellAction.performed -= OnZoom;
            scrollWhellAction.canceled -= OnZoom;
            scrollWhellAction.Disable();
        }

        // Очистка ссылки на камеру
        Game_Manager.instance._moveModule._mainCamera = null;
        cinemachine3rdPersonFollow = null;
    }

    private void SwitchCamera(InputAction.CallbackContext context)
    {
        if (Game_Manager.instance == null || Game_Manager.instance._moveModule == null)
            return;
        bool state = !Game_Manager.instance._moveModule.CameraShiftLock;

        CameraLockImage.SetActive(state);
        Cursor.lockState = state? CursorLockMode.Locked: CursorLockMode.None;
        Cursor.visible = !state;
        Game_Manager.instance._moveModule.CameraShiftLock = state;
        float Smoth = !state? .125f:0f;
        Game_Manager.instance._moveModule.minSmoothing = Smoth;
        Game_Manager.instance._moveModule.maxSmoothing = Smoth;
    }
    private void OnZoom(InputAction.CallbackContext context)
    {
        if (virtualLookCamera == null)
        {
            Debug.LogWarning("Virtual Camera is null. Ensure it is assigned or not destroyed.");
            return;
        }
        else if (cinemachine3rdPersonFollow == null)
        {
            Debug.LogWarning("Orbital Transposer is null. Ensure the Virtual Camera has the component.");
            return;
        }

        float directionf = context.ReadValue<Vector2>().y;
        cinemachine3rdPersonFollow.CameraDistance = Mathf.Clamp(cinemachine3rdPersonFollow.CameraDistance - (directionf * _zoomSpeed), MinZoom, MaxZoom);
    }

    public Transform Cameratarget; 
    private void RotateToTrue(InputAction.CallbackContext context) => OnRotate = true;
    private void RotateToFalse(InputAction.CallbackContext context) => OnRotate = false;

    public void RotateHandle()
    {
        // Вращение камеры по горизонтали и вертикали
        Cameratarget.rotation *= Quaternion.AngleAxis(look.x * rotationPower, Vector3.up);   // Горизонтальное вращение
        Cameratarget.rotation *= Quaternion.AngleAxis(-look.y * rotationPower, Vector3.right); // Вертикальное вращение

        // Ограничение угла по вертикальной оси (X)
        var localEulerAngles = Cameratarget.localEulerAngles;
        localEulerAngles.z = 0; // Убираем нежелательное вращение по оси Z

        float clampedX = localEulerAngles.x > 180 ? localEulerAngles.x - 360 : localEulerAngles.x; // Приводим угол в диапазон -180...180
        localEulerAngles.x = Mathf.Clamp(clampedX, -30, 60); // Ограничиваем угол по оси X
        Cameratarget.localEulerAngles = localEulerAngles;

        // Плавное стабилизирующее вращение
        Cameratarget.rotation = Quaternion.Lerp(Cameratarget.rotation, nextRotation, Time.deltaTime * rotationLerp);

        // Рассчёт движения камеры
        nextPosition = (Vector2.Distance(Vector2.zero, move) > 0.01f)
            ? (Cameratarget.forward * move.y + Cameratarget.right * move.x).normalized
            : Vector3.zero;
    }


    private void OnLook(InputAction.CallbackContext context)
    {
        if(OnRotate ||(CameraLockImage!=null&& CameraLockImage.activeSelf))
        {
            look = context.ReadValue<Vector2>();
            return;
        }else
        if(Vector2.Distance(Vector2.zero, look) > 0)
        {
            look = Vector2.Lerp(look, Vector2.zero, .15f );
        }
    }


    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnJumpStart(InputAction.CallbackContext context)
    {
        Game_Manager.instance._moveModule.Jumping = true;
    }

    private void OnJumpEnd(InputAction.CallbackContext context)
    {
        Game_Manager.instance._moveModule.Jumping = false;
    }
    public override void OnDestroy()
    {
        Deinit();
        Debug.Log("Deinited");
    }
}
