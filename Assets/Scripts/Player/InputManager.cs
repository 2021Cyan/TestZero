using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance;

    public static PlayerInput PlayerInput;
    
    public Vector3 MoveInput { get; private set; }
    public Vector3 MouseInput { get; private set; }
    public bool MenuInput { get; private set; }
    public bool MenuUIInput { get; private set; }
    public bool AimInput { get; private set; }
    public bool ClickInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DodgeInput { get; private set; }
    public bool ReloadInput { get; private set; }
    public bool BulletTimeInput { get; private set; }

    // make some set methods for the inputs
    public void SetMoveInput(Vector3 moveInput)
    {
        MoveInput = moveInput;
    }

    public void SetMouseInput(Vector3 mouseInput)
    {
        MouseInput = mouseInput;
    }

    public void SetMenuInput(bool menuInput)
    {
        MenuInput = menuInput;
    }

    public void SetMenuUIInput(bool menuUIInput)
    {
        MenuUIInput = menuUIInput;
    }

    public void SetAimInput(bool aimInput)
    {
        AimInput = aimInput;
    }

    public void SetClickInput(bool clickInput)
    {
        ClickInput = clickInput;
    }

    public void SetJumpInput(bool jumpInput)
    {
        JumpInput = jumpInput;
    }

    public void SetDodgeInput(bool dodgeInput)
    {
        DodgeInput = dodgeInput;
    }

    public void SetReloadInput(bool reloadInput)
    {
        ReloadInput = reloadInput;
    }

    public void SetBulletTimeInput(bool bulletTimeInput)
    {
        BulletTimeInput = bulletTimeInput;
    }

    private InputAction _moveInputAction;
    private InputAction _mouseInputAction;
    private InputAction _menuAction;
    private InputAction _menuUIAction;
    private InputAction _aimAction;
    private InputAction _clickAction;
    private InputAction _jumpAction;
    private InputAction _dodgeAction;
    private InputAction _reloadAction;
    private InputAction _bulletTimeAction;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        PlayerInput = GetComponent<PlayerInput>();

        _menuAction = PlayerInput.actions["Menu"];
        _menuUIAction = PlayerInput.actions["MenuUI"];
        _moveInputAction = PlayerInput.actions["Move"];

        _mouseInputAction = PlayerInput.actions["Look"];
        _aimAction = PlayerInput.actions["Aim"];
        _clickAction = PlayerInput.actions["Shoot"];
        _jumpAction = PlayerInput.actions["Jump"];
        _dodgeAction = PlayerInput.actions["Dodge"];
        _reloadAction = PlayerInput.actions["Reload"];
        _bulletTimeAction = PlayerInput.actions["BulletTime"];
    }

    // Update is called once per frame
    private void Update()
    {

        MoveInput = _moveInputAction.ReadValue<Vector2>();
        MouseInput = _mouseInputAction.ReadValue<Vector2>();
        MenuInput = _menuAction.WasPressedThisFrame();
        MenuUIInput = _menuUIAction.WasPressedThisFrame();
        AimInput = _aimAction.IsPressed();
        ClickInput = _clickAction.IsPressed();
        JumpInput = _jumpAction.WasPressedThisFrame();
        DodgeInput = _dodgeAction.WasPressedThisFrame();
        ReloadInput = _reloadAction.WasPressedThisFrame();
        BulletTimeInput = _bulletTimeAction.WasPressedThisFrame();
    }
}
