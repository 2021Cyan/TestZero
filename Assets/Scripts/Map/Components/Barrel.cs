using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Barrel : Interactable
{
    // Public attributes
    public int Reward;
    public GameObject Prompt;
    public float Likelihood = 1f;

    // Private attributes
    private bool _used = false;
    private bool _playerIsNear = false;
    private AudioManager _audio;
    private InputManager _input;
    private Light2D _light;

    // Behaviour
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_used && other.CompareTag("Player"))
        {
            _playerIsNear = true;
            ShowPrompt(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!_used && other.CompareTag("Player"))
        {
            _playerIsNear = false;
            ShowPrompt(false);
        }
    }

    void Awake()
    {
        // Destory self based on likelihood
        if (Random.value > Likelihood)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Hide prompt
        ShowPrompt(false);
        _audio = AudioManager.Instance;
        _input = InputManager.Instance;
        _light = GetComponent<Light2D>();

        if (_input != null)
        {
            _input.OnInteractPressed += HandleBarrelInteract;
        }
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.OnInteractPressed -= HandleBarrelInteract;
        }
    }

    private void HandleBarrelInteract()
    {
        if (_playerIsNear && !_used)    
        {
            _player.GetComponent<PlayerController>().AddResource(Reward);

            // Update appearance
            gameObject.GetComponent<Renderer>().material.color = Color.grey;

            // Remember interaction
            enabled = false;
            _used = true;
            _audio.SetParameterByName("Shop", 3);
            _audio.PlayOneShot(_audio.Shop);
            GetComponent<ParticleSystem>().Stop();
            _light.enabled = false;
            ShowPrompt(false);
        }
    }

    private void ShowPrompt(bool show)
    {
        if (show)
        {
            // Display "Press E to interact"
            Prompt.SetActive(true);
        }
        else
        {
            // Hide prompt text
            Prompt.SetActive(false);
        }
    }
}
