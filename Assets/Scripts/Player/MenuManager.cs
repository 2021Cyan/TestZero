using UnityEngine;
using FMODUnity;
public class MenuManager : MonoBehaviour
{
    public static bool IsPaused = false;
    public static Vector2 BeforePausePosition;
    public GameObject PauseMenuUI;
    public GameObject VolumeMenuUI;
    private InputManager _input;
    private AudioManager _audio;
    private void Start()
    {
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
    }

    private void Update()
    {
        if (_input.MenuInput || _input.MenuUIInput)
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
        _audio.PlayOneShotMenuOpen();
        PauseMenuUI.SetActive(false);
        VolumeMenuUI.SetActive(false);
        InputManager.Input.UI.Disable();
        InputManager.Input.Player.Enable();
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void Pause()
    {
        Cursor.visible = true;
        _audio.PlayOneShotMenuOpen();
        PauseMenuUI.SetActive(true);
        BeforePausePosition = _input.MouseInput;
        InputManager.Input.Player.Disable();
        InputManager.Input.UI.Enable();
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