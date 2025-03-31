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
    public bool PlayerIsNear(Vector3 selfPosition)
    {
        if (_player != null)
        {
            return Vector3.Distance(_player.transform.position, selfPosition) < ActivationDistance;
        }
        else
        {
            return false;
        }
    }
}
