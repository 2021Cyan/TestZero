using UnityEngine;

public class Barrel : Interactable
{
    // Public attributes
    public int Reward;
    public GameObject _prompt;

    // Private attributes
    private bool _used = false;
    private bool _playerIsNear = false;

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
        // Hide prompt
        ShowPrompt(false);
    }
    
    private void Update()
    {
        // Allow interactions if player is close enough and barrel hasn't been used
        if (_playerIsNear && !_used)
        {
            // Show prompt
            ShowPrompt(true);

            // Handle interaction
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Reward player
                _player.GetComponent<PlayerController>().AddResource(Reward);

                // Update appearance
                // TODO
                gameObject.GetComponent<Renderer>().material.color = Color.grey;

                // Remember interaction
                enabled = false;
                _used = true;
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
            _prompt.SetActive(true);
        }
        else
        {
            // Hide prompt text
            _prompt.SetActive(false);
        }
    }
}
