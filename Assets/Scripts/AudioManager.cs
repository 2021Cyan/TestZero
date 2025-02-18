using UnityEngine;
using FMODUnity;



public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public EventReference Music;

    [Header("Ambience")]
    public EventReference Ambience;

    [Header("Player_SFX")]
    public EventReference Aim;
    public EventReference Shoot;
    public EventReference Reload;
    public EventReference Hit;
    public EventReference Dodge;
    public EventReference JumpGroan;
    public EventReference Death;
    public EventReference Pickup;
    public EventReference Footstep;
    public EventReference Moonwalk;
    public EventReference AirDash;


    [Header("UI_SFX")]
    public EventReference UIClick;
    public EventReference UIHover;
    public EventReference MenuOpen;

    public static AudioManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayOneShot(EventReference sound, Vector3 p)
    {
        RuntimeManager.PlayOneShot(sound, p);
    }
    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void PlayOneShotUIClick()
    {
        RuntimeManager.PlayOneShot(UIClick);
    }
    public void PlayOneShotUIHover()
    {
        RuntimeManager.PlayOneShot(UIHover);
    }
    public void PlayOneShotMenuOpen()
    {
        RuntimeManager.PlayOneShot(MenuOpen);
    }
}
