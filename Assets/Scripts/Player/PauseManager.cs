using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PauseManager : MonoBehaviour
{
    public static bool IsPaused = false;
    public static Vector3 beforePausePosition;
    public GameObject PauseMenuUI;
    public GameObject VolumeMenuUI;
    private static InputManager _Input;

    private void Start()
    {
        _Input = InputManager.Instance;     
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
        PauseMenuUI.SetActive(false);
        VolumeMenuUI.SetActive(false);
        InputManager.PlayerInput.actions.FindActionMap("UI").Disable();
        InputManager.PlayerInput.actions.FindActionMap("Player").Enable();
        Time.timeScale = 1f;
        
        
        // InputManager.PlayerInput.currentActionMap = InputManager.PlayerInput.actions.FindActionMap("Player");
        IsPaused = false;
    }

    public void Pause()
    {
    
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
    public void Quid()
    {
        Application.Quit();
    }
}