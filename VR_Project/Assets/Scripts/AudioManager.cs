﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

[System.Serializable]
public class Sound
{
    public string m_name;

    [HideInInspector]
    public AudioClip m_clip;
    public AudioSource m_source;

    public bool m_playOnAwake;
    public bool m_loop;

    [Range(0f, 1f)]
    public float m_volume;
    [Range(0f, 1f)]
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
        if (m_manager = null)
        {
            m_manager = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
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
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == name);

        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return;
        }

        soundToPlay.m_source.Play();
    }
}