/*
* File: AudioManager.cs
*
* Author: Mara Dusevic (s200494@students.aie.edu.au)
* Date Created: Friday 11 June 2021
* Date Last Modified: Friday 18 June 2021
*
* Appears in every scene and contains all audio files.
* Can be called in all scripts to play and stop audio.
*
*/

using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public string m_name; // The name of the audio clip

    public AudioClip m_clip; // The audio clip
    [HideInInspector]
    public AudioSource m_source; // The audio's set source

    // Used to set the values on the audio source components
    public bool m_playOnAwake;
    public bool m_loop; // Loops audio

    [Range(0f, 1f)]
    public float m_volume; // Changes volume of audio
    [Range(-3f, 3f)]
    public float m_pitch; // Changes pitch of audio
    [Range(0f, 1f)]
    public float m_spatialBlend; // Changes spatial blend for 2D to 3D
}

public class AudioManager : MonoBehaviour
{
    #region Properties

    // Array of all sounds attached to manager
    [SerializeField]
    public Sound[] m_sounds;

    // Current instance of manager in scene
    [HideInInspector]
    public static AudioManager m_manager;

    #endregion

    // Awake Function
    private void Awake()
    {
        // Sets all audio sources settings for components
        SetManagerSounds();
    }

    // Sets sound settings for audio sources components on manager
    private void SetManagerSounds()
    {
        foreach (Sound s in m_sounds)
        {
            s.m_source = gameObject.AddComponent<AudioSource>();
            SetSoundSettings(s);
        }
    }

    // Sets the sounds indiviual settings
    private void SetSoundSettings(Sound a_sound)
    {
        a_sound.m_source.clip = a_sound.m_clip;

        a_sound.m_source.playOnAwake = a_sound.m_playOnAwake;
        a_sound.m_source.loop = a_sound.m_loop;

        a_sound.m_source.volume = a_sound.m_volume;
        a_sound.m_source.pitch = a_sound.m_pitch;
        a_sound.m_source.spatialBlend = a_sound.m_spatialBlend;
    }

    // Override function to set sound settings on a specific source
    private void SetSoundSettings(AudioSource a_source, Sound a_sound)
    {
        a_source.clip = a_sound.m_clip;

        a_source.playOnAwake = a_sound.m_playOnAwake;
        a_source.loop = a_sound.m_loop;

        a_source.volume = a_sound.m_volume;
        a_source.pitch = a_sound.m_pitch;
        a_source.spatialBlend = a_sound.m_spatialBlend;
    }

    // Plays sound on the audio manager
    public void PlaySound (string a_name)
    {
        // Finds the sound in the all sounds array
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        // If sound cannot be found
        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return;
        }

        // Plays sound on audio manager's component
        soundToPlay.m_source.Play();
    }

    // Override function to play sound at a specfic location
    public void PlaySound(string a_name, GameObject a_source)
    {
        // Finds the sound in the all sounds array
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        // If sound cannot be found
        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return;
        }

        // Sets given audio's audio source
        GameObject soundObject = new GameObject(a_name);
        soundObject.transform.position = a_source.transform.position;
        AudioSource source = soundObject.AddComponent<AudioSource>();

        // Deletes sound in scene to stop stacking up
        Destroy(soundObject, 5f);
        SetSoundSettings(source, soundToPlay);
        source.Play();
    }

    // Stops sound on the audio manager
    public void StopPlaying (string a_name)
    {
        // Finds the sound in the all sounds array
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        // If sound cannot be found
        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return;
        }

        // Stops playing sound on audio manager's component
        soundToPlay.m_source.Stop();
    }

    // Override function to stop playing audio at a specfic location
    public void StopPlaying(string a_name, GameObject a_source)
    {
        // Finds all audio sources in scene
        AudioSource[] sources = FindObjectsOfType<AudioSource>();

        // If no sources found, return.
        if (sources == null)
        {
            Debug.LogWarning("No audio sources found.");
            return;
        }

        // Loop through all sources
        foreach (AudioSource source in sources)
        {
            // If the audio source's name is the same as the given name, stop that audio source and return
            if (source.gameObject.name == a_name && source.name == a_source.name)
            {
                source.Stop();
                return;
            }
        }

        // Warns if no audio source with given name could be found.
        Debug.LogWarning("No sound named " + a_name + " exists.");
    }

    // Returns a boolean on whether audio is playing
    public bool isPlaying(string a_name)
    {
        // Finds the sound in the all sounds array 
        Sound soundToPlay = Array.Find(m_sounds, sound => sound.m_name == a_name);

        // If sound cannot be found
        if (soundToPlay == null)
        {
            Debug.LogWarning("No sound named " + a_name + " exists.");
            return false;
        }

        // If sound is playing on audio source, return true.
        bool isPlaying = soundToPlay.m_source.isPlaying;
        return isPlaying;
    }
}