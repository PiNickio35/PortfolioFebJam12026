using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioMixMode
{
    LinearAudioSourceVolume, LinearMixerVolume, LogarithmicMixerVolume
}
public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private AudioMixMode _audioMixMode;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioMixer.SetFloat("Volume", Mathf.Log10(PlayerPrefs.GetFloat("Volume", 1)) * 20);
        }
    }

    public void OnChangeSlider(float value)
    {
        volumeText.SetText($"Volume: {value * 100:F0}");

        switch (_audioMixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                audioSource.volume = value;
                break;
            case AudioMixMode.LinearMixerVolume:
                audioMixer.SetFloat("Volume", -80+ value *80);
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                audioMixer.SetFloat("Volume", Mathf.Log10(value) * 20);
                break;
        }
        
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }
    
}
