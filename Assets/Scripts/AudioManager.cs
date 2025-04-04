using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using System.Collections;
using System.Collections.Generic;

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
    public EventReference Heal;

    [Header("Enemy_SFX")]
    public EventReference EnemyDeath;
    public EventReference Explosion;
    public EventReference Missile;
    public EventReference MissileLaunch;
    public EventReference EnemyHurt;
    public EventReference Laser;
    public EventReference EnemyFlying;

    [Header("SFX")]
    public EventReference Lava;
    public EventReference Shop;
    public EventReference Rarity;
    public EventReference BouncePad;

    [Header("UI_SFX")]
    public EventReference UIClick;
    public EventReference UIHover;
    public EventReference MenuOpen;

    public Bus MasterBus { get; private set; }
    public Bus SFXBus { get; private set; }
    public Bus MusicBus { get; private set; }

    public static AudioManager Instance { get; private set; }
    private Dictionary<EventInstance, Coroutine> activeCoroutines = new Dictionary<EventInstance, Coroutine>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            MasterBus = RuntimeManager.GetBus("bus:/");
            SFXBus = RuntimeManager.GetBus("bus:/SFX");
            MusicBus = RuntimeManager.GetBus("bus:/Music");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // RuntimeManager.CoreSystem.createSound
    }

    public void PlayOneShot(EventReference sound, Vector3? position = null)
    {
        var instance = RuntimeManager.CreateInstance(sound);

        if (position.HasValue)
        {
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(position.Value));
        }
        instance.start();
        instance.release();
    }

    public void SetParameterByName(string paraName, float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(paraName, value);
    }

    public float GetParameterByName(string paraName)
    {
        RuntimeManager.StudioSystem.getParameterByName(paraName, out float value);
        return value;
    }

    public void SetPitch(float pitch)
    {
        Bus masterBus = RuntimeManager.GetBus("bus:/");
        masterBus.getChannelGroup(out FMOD.ChannelGroup masterChannelGroup);
        masterChannelGroup.setPitch(pitch);
    }

    public EventInstance GetEventInstance(EventReference sound, GameObject gameObject = null)
    {
        var instance = RuntimeManager.CreateInstance(sound);
        if (gameObject != null)
        {
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        }
        return instance;
    }

    public void StopEvent(EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    public void StopFade(EventInstance instance)
    {
        if (activeCoroutines.TryGetValue(instance, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            activeCoroutines.Remove(instance);
        }
    }

    public void FadeInAudio(EventInstance instance, string parameterName, float targetValue, float duration)
    {
        StopFade(instance); // Stop any existing fade for this instance
        SetParameterByName(parameterName, GetParameterByName(parameterName));
        instance.start();

        var coroutine = StartCoroutine(FadeAudioCoroutine(instance, parameterName, 0f, targetValue, duration));
        activeCoroutines[instance] = coroutine;
    }

    public void FadeOutAudio(EventInstance instance, string parameterName, float duration)
    {
        StopFade(instance);
        var coroutine = StartCoroutine(FadeAudioCoroutine(instance, parameterName, GetParameterByName(parameterName), 0f, duration, true));
        activeCoroutines[instance] = coroutine;
    }

    private IEnumerator FadeAudioCoroutine(EventInstance instance, string parameterName, float startValue, float endValue, float duration, bool stopOnComplete = false)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            UnityEngine.Debug.Log($"Setting {parameterName} to {currentValue}");
            SetParameterByName(parameterName, currentValue);
            yield return null; // Ensure the coroutine yields control back to Unity
        }
        SetParameterByName(parameterName, endValue);

        if (stopOnComplete)
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }

        activeCoroutines.Remove(instance); // Remove the coroutine from the dictionary when done
    }

}
