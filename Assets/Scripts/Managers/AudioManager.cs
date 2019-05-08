using UnityEngine.Audio;
using System;
using UnityEngine;

//Based on Brackeys tutorial on how to create an AudioManager https://www.youtube.com/watch?v=6OT43pvUyfY
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private Sound[] _sounds;

    [SerializeField]
    private AudioSource _music;

    protected override void Awake()
    {
        base.Awake();

        foreach (var sound in _sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;
			sound.source.outputAudioMixerGroup = sound.mixerGroup;
        }      
    }

    public void Play(string sound)
    {
        Sound s = Array.Find(_sounds, item => item.name == sound);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;

        s.source.Play();
    }

    public void PauseMusic()
    {
        _music.Pause();
    }

    public void ResumeMusic()
    {
        _music.UnPause();
    }

}

[System.Serializable]
public class Sound
{
    public string name;
    public bool loop = false;

    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    [HideInInspector]
    public AudioSource source;

    [Range(0f, 1f)]
    public float volume = .75f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

}
