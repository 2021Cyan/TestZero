
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public static InputMap _Input;
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

        _Input.Enable();
        _Input.Player.Move.performed += SetMoveInput;
        _Input.Player.Move.canceled += SetMoveInput;

        _Input.Player.Look.performed += SetMouseInput;
        _Input.Player.Look.canceled += SetMouseInput;

        _Input.Player.Shoot.started += SetClickInput;
        _Input.Player.Shoot.canceled += SetClickInput;
        
        _Input.Player.Menu.started += SetMenuInput;
        _Input.Player.Menu.canceled += SetMenuInput;

        _Input.UI.MenuUI.started += SetMenuUIInput;
        _Input.UI.MenuUI.canceled += SetMenuUIInput;

        _Input.Player.Aim.started += SetAimInput;
        _Input.Player.Aim.canceled += SetAimInput;

        _Input.Player.Jump.started += SetJumpInput;
        _Input.Player.Jump.canceled += SetJumpInput;

        _Input.Player.Dodge.started += SetDodgeInput;
        _Input.Player.Dodge.canceled += SetDodgeInput;

        _Input.Player.Reload.started += SetReloadInput;
        _Input.Player.Reload.canceled += SetReloadInput;

        _Input.Player.BulletTime.started += SetBulletTimeInput;
        _Input.Player.BulletTime.canceled += SetBulletTimeInput;

        _Input.Player.Interact.started += SetInteractInput;
        _Input.Player.Interact.canceled += SetInteractInput;

        _Input.Player.Reset.started += SetResetInput;
        _Input.Player.Reset.canceled += SetResetInput;

        _Input.Player.F.started += SetFInput;
        _Input.Player.F.canceled += SetFInput;
        
        _Input.Player.G.started += SetGInput;
        _Input.Player.G.canceled += SetGInput;
    }

    private void OnDisable()
    {
        _Input.Player.Move.performed -= SetMoveInput;
        _Input.Player.Move.canceled -= SetMoveInput;

        _Input.Player.Shoot.started -= SetClickInput;
        _Input.Player.Shoot.canceled -= SetClickInput;

        _Input.Player.Look.performed -= SetMouseInput;
        _Input.Player.Look.canceled -= SetMouseInput;

        _Input.Player.Menu.started -= SetMenuInput;
        _Input.Player.Menu.canceled -= SetMenuInput;

        _Input.UI.MenuUI.started -= SetMenuUIInput;
        _Input.UI.MenuUI.canceled -= SetMenuUIInput;

        _Input.Player.Aim.started -= SetAimInput;
        _Input.Player.Aim.canceled -= SetAimInput;

        _Input.Player.Jump.started -= SetJumpInput;
        _Input.Player.Jump.canceled -= SetJumpInput;

        _Input.Player.Dodge.started -= SetDodgeInput;
        _Input.Player.Dodge.canceled -= SetDodgeInput;

        _Input.Player.Reload.started -= SetReloadInput;
        _Input.Player.Reload.canceled -= SetReloadInput;

        _Input.Player.BulletTime.started -= SetBulletTimeInput;
        _Input.Player.BulletTime.canceled -= SetBulletTimeInput;

        _Input.Player.Interact.started -= SetInteractInput;
        _Input.Player.Interact.canceled -= SetInteractInput;

        _Input.Player.Reset.started -= SetResetInput;
        _Input.Player.Reset.canceled -= SetResetInput;

        _Input.Player.F.started -= SetFInput;
        _Input.Player.F.canceled -= SetFInput;

        _Input.Player.G.started -= SetGInput;
        _Input.Player.G.canceled -= SetGInput;

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
        _Input = new InputMap();
    }

    // Update is called once per frame
    private void Update()
    {
        MoveInput = _Input.Player.Move.ReadValue<Vector2>();
        MouseInput = _Input.Player.Look.ReadValue<Vector2>();
        MenuInput = _Input.Player.Menu.WasPressedThisFrame();
        MenuUIInput = _Input.UI.MenuUI.WasPressedThisFrame();
        AimInput = _Input.Player.Aim.IsPressed();
        ClickInput = _Input.Player.Shoot.IsPressed();
        JumpInput = _Input.Player.Jump.WasPressedThisFrame();
        DodgeInput = _Input.Player.Dodge.WasPressedThisFrame();
        ReloadInput = _Input.Player.Reload.WasPressedThisFrame();
        BulletTimeInput = _Input.Player.BulletTime.WasPressedThisFrame();
        InteractInput = _Input.Player.Interact.WasPressedThisFrame();
        ResetInput = _Input.Player.Reset.WasPressedThisFrame();

        // We need better names for these
        FInput = _Input.Player.F.IsPressed();
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
        GInput = _Input.Player.G.IsPressed();  
    }
}
