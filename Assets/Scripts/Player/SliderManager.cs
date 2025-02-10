using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class SliderManager : MonoBehaviour
{
    private enum VolumeType
    {
        Master,
        Music,
        SFX
    }

    [Header("Type of Volume")]
    [SerializeField] private VolumeType _volumeType;

    private Slider volumeSlider;

    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private string GetVolumeKey()
    {
        switch (_volumeType)
        {
            case VolumeType.Master:
                return MASTER_VOLUME_KEY;
            case VolumeType.Music:
                return MUSIC_VOLUME_KEY;
            case VolumeType.SFX:
                return SFX_VOLUME_KEY;
            default:
                return MASTER_VOLUME_KEY;
        }
    }
    private void Awake()
    {
        volumeSlider = this.GetComponentInChildren<Slider>();
        volumeSlider.value = PlayerPrefs.GetFloat(GetVolumeKey(), 0.8f) * volumeSlider.maxValue;
        PlayerPrefs.SetFloat(GetVolumeKey(), volumeSlider.value);
    }

    public void UpdateVolume()
    {
        float volume = volumeSlider.value / volumeSlider.maxValue;
        switch (_volumeType)
        {
            case VolumeType.Master:
                RuntimeManager.GetBus("bus:/").setVolume(volume);
                PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volumeSlider.value);
                break;
            case VolumeType.Music:
                RuntimeManager.GetBus("bus:/Music").setVolume(volume);
                PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volumeSlider.value);
                break;
            case VolumeType.SFX:
                RuntimeManager.GetBus("bus:/SFX").setVolume(volume);
                PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volumeSlider.value);
                break;
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}