using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public static InputMap Input;
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public Vector2 MouseInput { get; private set; } = Vector2.zero;
    public bool MenuInput { get; private set; } = false;
    public bool MenuUIInput { get; private set; } = false;
    public bool AimInput { get; private set; } = false;
    public bool ClickInput { get; private set; } = false;
    public bool JumpInput { get; private set; } = false;
    public bool DodgeInput { get; private set; } = false;
    public bool ReloadInput { get; private set; } = false;
    public bool BulletTimeInput { get; private set; } = false;
    public bool InteractInput { get; private set; } = false;
    public bool FInput { get; private set; } = false;
    public bool GInput { get; private set; } = false;
    public bool ResetInput { get; private set; } = false;

    // Event for interact input
    // This event is triggered when the interact button is pressed
    public event System.Action OnInteractPressed;
    public event System.Action OnMenuPressed;
    public event System.Action OnResetPressed;

    private double _fPressTime = 0f;
    private double _gPressTime = 0f;

    private void OnEnable()
    {
        Input.Enable();
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // Input.Enable();
            EventEnable();
        }
    }

    private void EventEnable()
    {
        Input.Player.Move.performed += SetMoveInput;
        Input.Player.Move.canceled += SetMoveInput;

        Input.Player.Look.performed += SetMouseInput;
        Input.Player.Look.canceled += SetMouseInput;

        Input.Player.Shoot.started += SetClickInput;
        Input.Player.Shoot.canceled += SetClickInput;

        Input.Player.Menu.started += SetMenuInput;
        Input.Player.Menu.canceled += SetMenuInput;

        // Input.UI.MenuUI.started += SetMenuUIInput;
        // Input.UI.MenuUI.canceled += SetMenuUIInput;

        Input.Player.Aim.started += SetAimInput;
        Input.Player.Aim.canceled += SetAimInput;

        Input.Player.Jump.started += SetJumpInput;
        Input.Player.Jump.canceled += SetJumpInput;

        Input.Player.Dodge.started += SetDodgeInput;
        Input.Player.Dodge.canceled += SetDodgeInput;

        Input.Player.Reload.started += SetReloadInput;
        Input.Player.Reload.canceled += SetReloadInput;

        Input.Player.BulletTime.started += SetBulletTimeInput;
        Input.Player.BulletTime.canceled += SetBulletTimeInput;

        Input.Player.Interact.started += SetInteractInput;
        Input.Player.Interact.canceled += SetInteractInput;

        Input.Player.Reset.started += SetResetInput;
        Input.Player.Reset.canceled += SetResetInput;

        Input.Player.F.started += SetFInput;
        Input.Player.F.canceled += SetFInput;

        Input.Player.G.started += SetGInput;
        Input.Player.G.canceled += SetGInput;
    }

    private void OnDisable()
    {
        Input.Disable();
        EventDisable();
    }

    private void EventDisable()
    {
        Input.Player.Move.performed -= SetMoveInput;
        Input.Player.Move.canceled -= SetMoveInput;

        Input.Player.Shoot.started -= SetClickInput;
        Input.Player.Shoot.canceled -= SetClickInput;

        Input.Player.Look.performed -= SetMouseInput;
        Input.Player.Look.canceled -= SetMouseInput;

        Input.Player.Menu.started -= SetMenuInput;
        Input.Player.Menu.canceled -= SetMenuInput;

        // Input.UI.MenuUI.started -= SetMenuUIInput;
        // Input.UI.MenuUI.canceled -= SetMenuUIInput;

        Input.Player.Aim.started -= SetAimInput;
        Input.Player.Aim.canceled -= SetAimInput;

        Input.Player.Jump.started -= SetJumpInput;
        Input.Player.Jump.canceled -= SetJumpInput;

        Input.Player.Dodge.started -= SetDodgeInput;
        Input.Player.Dodge.canceled -= SetDodgeInput;

        Input.Player.Reload.started -= SetReloadInput;
        Input.Player.Reload.canceled -= SetReloadInput;

        Input.Player.BulletTime.started -= SetBulletTimeInput;
        Input.Player.BulletTime.canceled -= SetBulletTimeInput;

        Input.Player.Interact.started -= SetInteractInput;
        Input.Player.Interact.canceled -= SetInteractInput;

        Input.Player.Reset.started -= SetResetInput;
        Input.Player.Reset.canceled -= SetResetInput;

        Input.Player.F.started -= SetFInput;
        Input.Player.F.canceled -= SetFInput;

        Input.Player.G.started -= SetGInput;
        Input.Player.G.canceled -= SetGInput;
    }

    private void SetMoveInput(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void SetClickInput(InputAction.CallbackContext ctx)
    {
        ClickInput = ctx.started;
    }

    private void SetMouseInput(InputAction.CallbackContext ctx)
    {
        MouseInput = ctx.ReadValue<Vector2>();
        // Debug.Log("Mouse Input: " + MouseInput);
    }

    public void SetMouseInput(Vector2 v)
    {
        MouseInput = v;
    }

    private void SetAimInput(InputAction.CallbackContext ctx)
    {
        AimInput = ctx.started;
    }

    private void SetJumpInput(InputAction.CallbackContext ctx)
    {
        JumpInput = ctx.started;
    }

    private void SetDodgeInput(InputAction.CallbackContext ctx)
    {
        DodgeInput = ctx.started;
    }

    private void SetReloadInput(InputAction.CallbackContext ctx)
    {
        ReloadInput = ctx.started;
    }

    private void SetBulletTimeInput(InputAction.CallbackContext ctx)
    {
        BulletTimeInput = ctx.started;
    }

    private void SetInteractInput(InputAction.CallbackContext ctx)
    {
        InteractInput = ctx.started;
        if (InteractInput)
        {
            OnInteractPressed?.Invoke();
        }
    }

    private void SetMenuInput(InputAction.CallbackContext ctx)
    {
        MenuInput = ctx.started;
        if (MenuInput)
        {
            OnMenuPressed?.Invoke();
        }
    }

    private void SetResetInput(InputAction.CallbackContext ctx)
    {
        ResetInput = ctx.started;
        if (ResetInput)
        {
            OnResetPressed?.Invoke();
        }
    }

    public double GetFPressTime()
    {
        return _fPressTime;
    }

    private void SetFInput(InputAction.CallbackContext ctx)
    {
        FInput = ctx.started;
        if (!FInput)
        {
            _fPressTime = 0f;
        }
        else
        {
            _fPressTime = ctx.startTime;
        }
    }

    public double GetGPressTime()
    {
        return _gPressTime;
    }

    private void SetGInput(InputAction.CallbackContext ctx)
    {
        GInput = ctx.started;
        if (!GInput)
        {
            _gPressTime = 0f;
        }
        else
        {
            _gPressTime = ctx.startTime;
        }
    }

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
        }
        Input = new InputMap();
    }

    // Update is called once per frame
    private void Update()
    {
        MouseInput = Input.Player.Look.ReadValue<Vector2>();
    }

    public void EnableInput()
    {
        Input.Enable();
        EventEnable();
    }

    public void DisableInput()
    {
        Input.Disable();
        EventDisable();
    }

    public void EnableMenuInput()
    {
        EventDisable();
        Input.Player.Menu.started += SetMenuInput;
        Input.Player.Menu.canceled += SetMenuInput;
    }
    // public void DisableMenuInput()
    // {
    //     EventEnable();
    // }
}