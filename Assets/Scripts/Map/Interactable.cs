using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Attributes
    protected GameObject _player;

    // Setters
    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    // Behaviour
    public void Awake()
    {
        
    }
}
