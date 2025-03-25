using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Attributes
    public float ActivationDistance;
    protected GameObject _player;

    // Setters
    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    // Behaviour
    public bool PlayerIsNear()
    {
        return Mathf.Abs(_player.transform.position.x - transform.position.x) < ActivationDistance;
    }
}
