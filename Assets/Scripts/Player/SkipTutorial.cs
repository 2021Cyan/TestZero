using EasyTransition;
using UnityEngine;

public class SkipTutorial : MonoBehaviour
{
    [SerializeField] private GameObject skipTutorialBarrel;
    private InputManager _input;
    private AudioManager _audio;
    private bool _playerIsNear = false;

    void Start()
    {
        if (PlayerPrefs.GetInt("SkipTutorial", 0) >= 1)
        {
            skipTutorialBarrel.SetActive(true);
        }
        else
        {
            skipTutorialBarrel.SetActive(false);
        }
        _input = InputManager.Instance;
        _input.OnInteractPressed += SkipTutorialBarrelInteract;
        _audio = AudioManager.Instance;
    }

    private void SkipTutorialBarrelInteract()
    {
        if (!_playerIsNear) return;
        _audio.SetParameterByName("Shop", 3);
        _audio.PlayOneShot(_audio.Shop);
        GameObject.FindWithTag("SceneTeleporter").GetComponent<SceneTeleporter>().LoadScene("MainGame");
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.OnInteractPressed -= SkipTutorialBarrelInteract;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsNear = true;
        }
    }
}
