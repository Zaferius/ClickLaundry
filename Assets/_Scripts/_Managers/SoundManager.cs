using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Instance Method
    public static SoundManager Instance;
    private void InstanceMethod()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            
            _audioSources = new List<AudioSource>();
            for (int i = 0; i < maxConcurrentSounds; i++)
            {
                AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
                _audioSources.Add(newAudioSource);
            }
        }
    }
    #endregion

    public List<AudioClip> soundClips;
    public int maxConcurrentSounds = 5;
    [HideInInspector] public List<AudioSource> _audioSources;


    public float specialFloat;
    private void Awake()
    {
        #region Instance Method
        InstanceMethod();
        #endregion
    }

    public void PlaySound(string soundName, float volume = 1, bool loop = false)
    {
        var soundClip = soundClips.Find(sound => sound.name == soundName);

        if (soundClip != null)
        {
            var availableSource = _audioSources.Find(source => !source.isPlaying);
            availableSource.pitch = 1;
            specialFloat = 0;
            
            if (availableSource != null)
            {
                availableSource.loop = loop;
                availableSource.volume = volume;
                availableSource.clip = soundClip;
                availableSource.Play();
            }
            else
            {
                Debug.LogWarning("No available AudioSources to play sound: " + soundName);
            }
        }
        else
        {
            Debug.LogError("Sound with the name " + soundName + " not found.");
        }
    }
    
    public void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            var availableSource = _audioSources.Find(source => !source.isPlaying);
            availableSource.pitch = 1;

            if (availableSource != null)
            {
                availableSource.volume = volume;
                availableSource.PlayOneShot(clip);
            }
        }
    }

    public void PlaySoundSpecial(string soundName, float volume = 1)
    {
        var soundClip = soundClips.Find(sound => sound.name == soundName);

        if (soundClip != null)
        {
            var availableSource = _audioSources.Find(source => !source.isPlaying);

            if (availableSource != null)
            {
                availableSource.volume = volume;
                availableSource.pitch += specialFloat;
                availableSource.PlayOneShot(soundClip);
            }
            else
            {
                Debug.LogWarning("No available AudioSources to play sound: " + soundName);
            }
        }
        else
        {
            Debug.LogError("Sound with the name " + soundName + " not found.");
        }
    }
    
    public void StopSound(string soundName)
    {
        var targetSource = _audioSources.Find(source =>
        {
            AudioClip clip;
            return source.isPlaying && (clip = source.clip) != null && clip.name == soundName;
        });

        if (targetSource == null) return;
        
        targetSource.loop = false;
        targetSource.Stop();
    }
}
