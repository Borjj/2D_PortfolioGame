using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool spatialize = true;  // Whether to use spatial audio
        [Range(1f, 50f)] public float maxDistance = 15f;  // Max distance to hear the sound
        [Range(1f, 10f)] public float minDistance = 1f;   // Distance for max volume
    }

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.5f;

    [Header("Sound Effects")]
    [SerializeField] private SoundEffect[] soundEffects;

    private AudioSource musicSource;
    private Dictionary<string, SoundEffect> soundDictionary;
    private List<AudioSource> sfxSources;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        // Setup music source (non-spatial)
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f; // 0 = 2D sound

        // Setup sound effects dictionary
        soundDictionary = new Dictionary<string, SoundEffect>();
        sfxSources = new List<AudioSource>();

        foreach (SoundEffect sound in soundEffects)
        {
            soundDictionary[sound.name] = sound;
        }

        PlayMusic();
    }

    public void PlaySound(string soundName, Vector3 position = default)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundEffect sound))
        {
            AudioSource source = GetAvailableSource();
            ConfigureAudioSource(source, sound, position);
            source.Play();
        }
    }

    private void ConfigureAudioSource(AudioSource source, SoundEffect sound, Vector3 position)
    {
        source.clip = sound.clip;
        source.volume = sound.volume;

        if (sound.spatialize)
        {
            source.spatialBlend = 1f; // 1 = 3D sound
            source.minDistance = sound.minDistance;
            source.maxDistance = sound.maxDistance;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.transform.position = position;
        }
        else
        {
            source.spatialBlend = 0f; // 0 = 2D sound
        }
    }

    private AudioSource GetAvailableSource()
    {
        AudioSource source = sfxSources.Find(s => !s.isPlaying);
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            sfxSources.Add(source);
        }
        return source;
    }

    public void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void RestartMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.Play();
        }
    }

    // Add this method to AudioManager.cs
    public AudioSource PlayLoopedSound(string soundName, Vector3 position = default)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundEffect sound))
        {
            AudioSource source = GetAvailableSource();
            ConfigureAudioSource(source, sound, position);
            source.loop = true; // Enable looping
            source.Play();
            return source;
        }
        return null;
    }

    // Add method to stop a specific audio source
    public void StopSound(AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
            source.loop = false;
        }
    }
}


/*
To add sound FX to things just use this in the Object Script:

private void PlayCollectSound()
{
    AudioManager.Instance?.PlaySound("Collect");
}

And then call the method when needed.
*/

