using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Slider = UnityEngine.UI.Slider;

public enum AudioMixMode
{
    LinearMixerVolume, LogarithmicMixerVolume
}
public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixMode audioMixMode;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Volume")) return;
        volumeText.SetText($"Volume: {PlayerPrefs.GetFloat("Volume", 1)*100:F0}");
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
        audioMixer.SetFloat("Volume", Mathf.Log10(PlayerPrefs.GetFloat("Volume", 1)) * 20);
    }

    public void OnChangeSlider(float value)
    {
        volumeText.SetText($"Volume: {value * 100:F0}");

        switch (audioMixMode)
        {
            case AudioMixMode.LinearMixerVolume:
                audioMixer.SetFloat("Volume", -80+ value *80);
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                audioMixer.SetFloat("Volume", Mathf.Log10(value) * 20);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }
    
}
