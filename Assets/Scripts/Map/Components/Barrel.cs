using TMPro;
using UnityEngine;

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

    // Behaviour
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_used && other.CompareTag("Player"))
        {
            // Allow interaction
            _playerIsNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!_used && other.CompareTag("Player"))
        {
            // Allow interaction
            _playerIsNear = false;
        }
    }

    void Start()
    {
        // Destory self based on likelihood
        if (Random.value > Likelihood)
        {
            Destroy(gameObject);
        }

        // Hide prompt
        ShowPrompt(false);
        _audio = AudioManager.Instance;
        _input = InputManager.Instance;
    }

    private void Update()
    {
        // Allow interactions if player is close enough and barrel hasn't been used
        if (_playerIsNear && !_used)
        {
            // Show prompt
            ShowPrompt(true);

            // Handle interaction
            if (_input.InteractInput)
            {
                // Reward player
                _player.GetComponent<PlayerController>().AddResource(Reward);

                // Update appearance
                // TODO
                gameObject.GetComponent<Renderer>().material.color = Color.grey;

                // Remember interaction
                enabled = false;
                _used = true;
                _audio.SetParameterByName("Shop", 3);
                _audio.PlayOneShot(_audio.Shop);
                GetComponent<ParticleSystem>().Stop();
                ShowPrompt(false);
            }
        }

        // Otherwise, don't show prompt
        else
        {
            ShowPrompt(false);
        }
    }

    private void ShowPrompt(bool show)
    {
        if (show)
        {
            // Display "Press E to interact"
            // TODO
            Prompt.SetActive(true);
        }
        else
        {
            // Hide prompt text
            Prompt.SetActive(false);
        }
    }
}
