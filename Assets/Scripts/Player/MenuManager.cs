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
    public GameObject[] VolumeSliders;
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

        VolumeMenuUI.SetActive(true);
        foreach (var slider in VolumeSliders)
        {
            slider.GetComponent<SliderManager>()?.UpdateVolume();
        }
        VolumeMenuUI.SetActive(false);
        _input.OnMenuPressed += PauseCheck;
    }

    private void OnDestroy()
    {
        _input.OnMenuPressed -= PauseCheck;
    }

    // private void Update()
    // {
    //     if (_input.MenuInput || _input.MenuUIInput)
    //     {
    //         PauseCheck();
    //     }
    // }

    public void PauseCheck()
    {
        _audio.PlayOneShot(_audio.MenuOpen);
        if (IsPaused)
        {
            IsPaused = false;
            _input.EnableInput();
            Resume();
        }
        else
        {
            IsPaused = true;
            BeforePausePosition = _input.MouseInput;
            _input.EnableMenuInput();
            Pause();
        }
    }

    public void Pause()
    {
        Cursor.visible = true;
        
        PauseMenuUI.SetActive(true);
        _timeScale = Time.timeScale;
        Time.timeScale = 0f;

    }

    public void Resume()
    {
        Cursor.visible = false;
        PauseMenuUI.SetActive(false);
        VolumeMenuUI.SetActive(false);
        Time.timeScale = _timeScale;
    }

    public void LoadMenu()
    {
        _playerController.Restart();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayOneShotUIClick()
    {
        _audio.PlayOneShot(_audio.UIClick);
    }

    public void PlayOneShotUIHover()
    {
        _audio.PlayOneShot(_audio.UIHover);
    }

    public void PlayOneShotMenuOpen()
    {
        _audio.PlayOneShot(_audio.MenuOpen);
    }
}