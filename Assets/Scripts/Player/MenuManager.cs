using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem;
public class MenuManager : MonoBehaviour
{
    public static bool IsPaused = false;
    public static Vector2 BeforePausePosition;
    public static MenuManager Instance;
    public GameObject PauseMenuUI;
    public GameObject VolumeMenuUI;
    private InputManager _input;
    private AudioManager _audio;
    private PlayerController _playerController;
    private float _timeScale;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
        _playerController = PlayerController.Instance;
        _timeScale = Time.timeScale;
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
            IsPaused = false;
            InputManager.Input.Enable();
            Resume();
        }
        else
        {
            IsPaused = true;
            BeforePausePosition = _input.MouseInput;
            InputManager.Input.Disable();
            InputManager.Input.UI.Enable();
            Pause();
        }
    }

    public void Pause()
    {
        Cursor.visible = true;
        _audio.PlayOneShotMenuOpen();
        PauseMenuUI.SetActive(true);
        _timeScale = Time.timeScale;
        Time.timeScale = 0f;
        
    }

    public void Resume()
    {
        Cursor.visible = false;
        _audio.PlayOneShotMenuOpen();
        PauseMenuUI.SetActive(false);
        VolumeMenuUI.SetActive(false);
        Time.timeScale = _timeScale;
    }

    public void LoadMenu()
    {
        _playerController.RestartGame();
    }

    public void Quit()
    {
        Application.Quit();
    }
}