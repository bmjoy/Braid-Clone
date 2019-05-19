using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class SoundMenu : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private GameObject SliderSFXVolume, SliderMusicVolume;

    public void Start()
    {
        SliderSFXVolume.GetComponent<Slider>().value = SFXVolume;
        SliderMusicVolume.GetComponent<Slider>().value = MusicVolume; 
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(SliderSFXVolume);
        AudioManager.Instance.Play(Sound.MENU);
    }

    public float SFXVolume
    {
        get
        {
            audioMixer.GetFloat("SFXVolume", out var value);
            return value;
        }
        set => audioMixer.SetFloat("SFXVolume", value);
    }

    public float MusicVolume
    {
        get
        {
            audioMixer.GetFloat("MusicVolume", out var value);
            return value;
        }
        set => audioMixer.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float volume) => SFXVolume = volume;

	public void SetMusicVolume(float volume) => MusicVolume = volume;

}
