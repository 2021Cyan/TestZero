
using UnityEngine;
using UnityEngine.InputSystem;

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

    double _fPressTime = 0f;
    double _gPressTime = 0f;
    private void OnEnable()
    {

        Input.Enable();
        Input.Player.Move.performed += SetMoveInput;
        Input.Player.Move.canceled += SetMoveInput;

        Input.Player.Look.performed += SetMouseInput;
        Input.Player.Look.canceled += SetMouseInput;

        Input.Player.Shoot.started += SetClickInput;
        Input.Player.Shoot.canceled += SetClickInput;

        Input.Player.Menu.started += SetMenuInput;
        Input.Player.Menu.canceled += SetMenuInput;

        Input.UI.MenuUI.started += SetMenuUIInput;
        Input.UI.MenuUI.canceled += SetMenuUIInput;

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
        Input.Player.Move.performed -= SetMoveInput;
        Input.Player.Move.canceled -= SetMoveInput;

        Input.Player.Shoot.started -= SetClickInput;
        Input.Player.Shoot.canceled -= SetClickInput;

        Input.Player.Look.performed -= SetMouseInput;
        Input.Player.Look.canceled -= SetMouseInput;

        Input.Player.Menu.started -= SetMenuInput;
        Input.Player.Menu.canceled -= SetMenuInput;

        Input.UI.MenuUI.started -= SetMenuUIInput;
        Input.UI.MenuUI.canceled -= SetMenuUIInput;

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

        //TODO: I am not sure if I remove this but it works okay
        // _Input.Disable();
    }

    // make some set methods for the inputs
    public void SetMoveInput(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    public void SetClickInput(InputAction.CallbackContext ctx)
    {
        ClickInput = ctx.started;
    }

    public void SetMouseInput(InputAction.CallbackContext ctx)
    {
        MouseInput = ctx.ReadValue<Vector2>();
    }

    public void SetMouseInput(Vector2 v)
    {
        MouseInput = v;
    }

    public void SetMenuInput(InputAction.CallbackContext ctx)
    {
        MenuInput = ctx.started;
    }

    public void SetMenuUIInput(InputAction.CallbackContext ctx)
    {
        MenuUIInput = ctx.started;
    }

    public void SetAimInput(InputAction.CallbackContext ctx)
    {
        AimInput = ctx.started;
    }

    public void SetJumpInput(InputAction.CallbackContext ctx)
    {
        JumpInput = ctx.started;
    }

    public void SetDodgeInput(InputAction.CallbackContext ctx)
    {
        DodgeInput = ctx.started;
    }

    public void SetReloadInput(InputAction.CallbackContext ctx)
    {
        ReloadInput = ctx.started;
    }

    public void SetBulletTimeInput(InputAction.CallbackContext ctx)
    {
        BulletTimeInput = ctx.started;
    }
    public void SetInteractInput(InputAction.CallbackContext ctx)
    {
        InteractInput = ctx.started;
    }
    public void SetResetInput(InputAction.CallbackContext ctx)
    {
        ResetInput = ctx.started;
    }
    public double GetFPressTime()
    {
        return _fPressTime;
    }
    public void SetFInput(InputAction.CallbackContext ctx)
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
    public void SetGInput(InputAction.CallbackContext ctx)
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
            return;
        }
        Input = new InputMap();
    }

    // Update is called once per frame
    private void Update()
    {
        MoveInput = Input.Player.Move.ReadValue<Vector2>();
        MouseInput = Input.Player.Look.ReadValue<Vector2>();
        MenuUIInput = Input.UI.MenuUI.WasPressedThisFrame();
        MenuInput = Input.Player.Menu.WasPressedThisFrame();
        AimInput = Input.Player.Aim.IsPressed();
        ClickInput = Input.Player.Shoot.IsPressed();
        JumpInput = Input.Player.Jump.WasPressedThisFrame();
        DodgeInput = Input.Player.Dodge.WasPressedThisFrame();
        ReloadInput = Input.Player.Reload.WasPressedThisFrame();
        BulletTimeInput = Input.Player.BulletTime.WasPressedThisFrame();
        InteractInput = Input.Player.Interact.WasPressedThisFrame();
        ResetInput = Input.Player.Reset.WasPressedThisFrame();

        // We need better names for these
        FInput = Input.Player.F.IsPressed();
        // if (_Input.Player.F.WasPressedThisFrame())
        // {
        //     Debug.Log("F was pressed this frame");
        // }

        // if (_Input.Player.F.IsPressed())
        // {
        //     Debug.Log("F is pressed");
        //     if (Time.fixedUnscaledTime - _fPressTime >= 3f)
        //     {
        //         Debug.Log("F has been held for 3 second");
        //     }
        // }
        GInput = Input.Player.G.IsPressed();
    }
}
