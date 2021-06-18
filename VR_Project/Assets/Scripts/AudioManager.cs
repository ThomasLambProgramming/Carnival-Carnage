using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

[System.Serializable]
public class Sound
{
    public string m_name;

    public AudioClip m_clip;
    [HideInInspector]
    public AudioSource m_source;

    public bool m_playOnAwake;
    public bool m_loop;

    [Range(0f, 1f)]
    public float m_volume;
    [Range(-3f, 3f)]
    public float m_pitch;
    [Range(0f, 1f)]
    public float m_spatialBlend;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public Sound[] m_sounds;

    [HideInInspector]
    public static AudioManager m_manager;

    private void Awake()
    {
        //if (m_manager == null)
        //{
        //    m_manager = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //DontDestroyOnLoad(gameObject);
        SetManagerSounds();
    }

    private void SetManagerSounds()
    {
        foreach (Sound s in m_sounds)
        {
            s.m_source = gameObject.AddComponent<AudioSource>();
            SetSoundSettings(s);
        }
    }

    private void SetSoundSettings(Sound a_sound)
    {
        a_sound.m_source.clip = a_sound.m_clip;

        a_sound.m_source.playOnAwake = a_sound.m_playOnAwake;
        a_sound.m_source.loop = a_sound.m_loop;

        a_sound.m_source.volume = a_sound.m_volume;
        a_sound.m_source.pitch = a_sound.m_pitch;
        a_sound.m_source.spatialBlend = a_sound.m_spatialBlend;
    }

    private void SetSoundSettings(AudioSource a_source, Sound a_sound)
    {
        a_source.clip = a_sound.m_clip;

        a_source.playOnAwake = a_sound.m_playOnAwake;
        a_source.loop = a_sound.m_loop;

        a_source.volume = a_sound.m_volume;
        a_source.pitch = a_sound.m_pitch;
        a_source.spatialBlend = a_sound.m_spatialBlend;
    }

    public void PlaySound (string a_name)
    {
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return;
        }

        soundToPlay.m_source.Play();
    }

    public void PlaySound(string a_name, GameObject a_source)
    {
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return;
        }

        GameObject soundObject = new GameObject(a_name);
        soundObject.transform.position = a_source.transform.position;
        AudioSource source = soundObject.AddComponent<AudioSource>();
        //they were not deleting in scene and were stacking up
        Destroy(soundObject, 5f);
        SetSoundSettings(source, soundToPlay);
        source.Play();
    }

    public void StopPlaying (string a_name)
    {
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return;
        }

        soundToPlay.m_source.Stop();
    }

    public void StopPlaying(string a_name, GameObject a_source)
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();

        if (sources == null)
        {
            Debug.LogWarning("No audio sources found.");
            return;
        }

        foreach (AudioSource source in sources)
        {
            if (source.gameObject.name == a_name && source.name == a_source.name)
            {
                source.Stop();
                return;
            }
        }

        Debug.LogWarning("No sound named " + a_name + " exists.");
    }

    public bool isPlaying(string a_name)
    {
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return false;
        }

        bool isPlaying = soundToPlay.m_source.isPlaying;
        return isPlaying;
    }
}