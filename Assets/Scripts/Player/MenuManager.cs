using UnityEngine;
using FMODUnity;
public class MenuManager : MonoBehaviour
{
    public static bool IsPaused = false;
    public static Vector2 beforePausePosition;
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
        InputManager._Input.UI.Disable();
        InputManager._Input.Player.Enable();
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void Pause()
    {
        Cursor.visible = true;
        _Audio.PlayOneShotMenuOpen();
        PauseMenuUI.SetActive(true);
        beforePausePosition = _Input.MouseInput;
        InputManager._Input.UI.Enable();
        InputManager._Input.Player.Disable();
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