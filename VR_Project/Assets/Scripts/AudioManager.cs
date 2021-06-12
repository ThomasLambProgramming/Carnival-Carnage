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
        SetSoundSettings();
    }

    private void SetSoundSettings()
    {
        foreach (Sound s in m_sounds)
        {
            s.m_source = gameObject.AddComponent<AudioSource>();
            s.m_source.clip = s.m_clip;

            s.m_source.playOnAwake = s.m_playOnAwake;
            s.m_source.loop = s.m_loop;

            s.m_source.volume = s.m_volume;
            s.m_source.pitch = s.m_pitch;
            s.m_source.spatialBlend = s.m_spatialBlend;
        }
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

        AudioSource.PlayClipAtPoint(soundToPlay.m_clip, a_source.transform.position);
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
        AudioSource[] sources = a_source.GetComponents<AudioSource>();

        if (sources == null)
        {
            Debug.LogWarning("No audio sources found.");
            return;
        }

        foreach (AudioSource source in sources)
        {
            if (source.name == a_name)
            {
                source.Stop();
                return;
            }
        }

        Debug.LogWarning("No sound named " + a_name + " exists.");
    }
}