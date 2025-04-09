using UnityEngine;
using FMODUnity;

public class LastRoom : MonoBehaviour
{
    private Collider2D _collider;


    void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RuntimeManager.GetBus("bus:/Music").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _collider.enabled = false;
        }
    }
}
