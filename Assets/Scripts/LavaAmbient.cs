using FMOD.Studio;
using Unity.Cinemachine;
using UnityEngine;

public class LavaAmbient : MonoBehaviour
{
    private AudioManager _audio;
    private InputManager _input;
    private EventInstance _lavaInstance;
    private CinemachineCamera _lavaCamera;
    [SerializeField] private float _fadeTime = 4f;
    [SerializeField] private bool cameraAdjust = false;

    void Start()
    {
        _audio = AudioManager.Instance;
        _input = InputManager.Instance;
        _input.OnMenuPressed += OnPaused;
        _lavaInstance = _audio.GetEventInstance(_audio.Lava, gameObject);
        _lavaCamera = GameObject.FindGameObjectWithTag("LavaCC").GetComponent<CinemachineCamera>();
    }

    private void OnPaused()
    {
        if (MenuManager.IsPaused)
        {
            _lavaInstance.setPaused(true);
        }
        else
        {
            _lavaInstance.setPaused(false);
        }
    }

    private void OnDestroy()
    {
        _input.OnMenuPressed -= OnPaused;
        if (_lavaInstance.isValid())
        {
            _audio.StopEvent(_lavaInstance);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (cameraAdjust)
            {
                _lavaCamera.Priority = 0;
                return;
            }
            if (!_lavaInstance.isValid())
                _lavaInstance = _audio.GetEventInstance(_audio.Lava, gameObject);
            _audio.FadeInAudio(_lavaInstance, "Vol", 1f, _fadeTime);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (cameraAdjust)
            {
                _lavaCamera.Priority = -2;
                return;
            }

            _audio.FadeOutAudio(_lavaInstance, "Vol", _fadeTime);
        }
    }
}
