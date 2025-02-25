using UnityEngine;
using FMODUnity;
public class MenuManager : MonoBehaviour
{
    public static bool IsPaused = false;
    public static Vector3 beforePausePosition;
    public GameObject PauseMenuUI;
    public GameObject VolumeMenuUI;
    private static InputManager _Input;
    private static AudioManager _Audio;
    private void Start()
    {
        _Input = InputManager.Instance;
        _Audio = AudioManager.Instance;
    }
    
    private void Update()
    {
        if (_Input.MenuInput || _Input.MenuUIInput)
        {
            PauseCheck();
        }
    }

    public void PauseCheck()
    {
        if (IsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        _Audio.PlayOneShotMenuOpen();
        PauseMenuUI.SetActive(false);
        VolumeMenuUI.SetActive(false);
        InputManager.PlayerInput.actions.FindActionMap("UI").Disable();
        InputManager.PlayerInput.actions.FindActionMap("Player").Enable();
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void Pause()
    {
        Cursor.visible = true;
        _Audio.PlayOneShotMenuOpen();
        PauseMenuUI.SetActive(true);
        beforePausePosition = _Input.MouseInput;
        InputManager.PlayerInput.actions.FindActionMap("Player").Disable();
        InputManager.PlayerInput.actions.FindActionMap("UI").Enable();
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}