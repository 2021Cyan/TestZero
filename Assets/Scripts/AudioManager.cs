using UnityEngine;
using FMODUnity;
using FMOD.Studio;



public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public EventReference Lobby;
    public EventReference Battle;

    [Header("Ambience")]
    public EventReference Ambience;

    [Header("Player_SFX")]
    public EventReference Pickup;
    public EventReference Aim;
    public EventReference Ricochet;
    public EventReference Shot;
    public EventReference Reload;
    public EventReference Dodge;
    public EventReference JumpGroan;
    public EventReference Footstep;
    public EventReference Moonwalk;
    public EventReference AirDash;
    public EventReference Death;
    public EventReference Hurt;

    [Header("Enemy_SFX")]
    public EventReference EnemyDeath;
    public EventReference Explosion;
    public EventReference EnemyHurt;
    public EventReference Laser;
    public EventReference EnemyFlying;

    [Header("SFX")]
    public EventReference Shop;
    public EventReference Rarity;
    public EventReference ButtonPress;

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
        RuntimeManager.WaitForAllSampleLoading();
    }

    public void PlayOneShot(EventReference sound, Vector3 p)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(p));
        instance.start();
        instance.release();
        // RuntimeManager.PlayOneShot(sound, p);
    }
    public void PlayOneShot(EventReference sound)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.start();
        instance.release();
        // RuntimeManager.PlayOneShot(sound);
    }

    public void SetParameterByName(string paraName, float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(paraName, value);
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

    public void SetPitch(float pitch)
    {
        Bus masterBus = RuntimeManager.GetBus("bus:/");
        masterBus.getChannelGroup(out FMOD.ChannelGroup masterChannelGroup);
        masterChannelGroup.setPitch(pitch);
    }
}
