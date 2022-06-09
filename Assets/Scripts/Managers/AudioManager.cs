using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource[] audioSources = new AudioSource[128];
    public AudioSource[] UnsafeAudioSources { get; } = new AudioSource[32];
    public float[] UnsafeAudioSourcesTimer { get; } = new float[32];
    private AudioSource ambienceAudioSource;
    private AudioSource musicAudioSource;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InvokeRepeating(nameof(DestroyAudioSources), 5.0f, 5.0f);
        
    }

    public void PlayMusic(AudioClip music, bool loop = true)
    {
        if (!musicAudioSource) musicAudioSource = gameObject.AddComponent<AudioSource>();
        if(musicAudioSource.isPlaying) musicAudioSource.Stop();
        musicAudioSource.volume = GameManager.Instance.gameOptions.masterVolume * GameManager.Instance.gameOptions.musicVolume;
        musicAudioSource.clip = music;
        musicAudioSource.loop = loop;
        musicAudioSource.Play();
    }

    public void StopMusic()
    {
        if (!musicAudioSource) return;
        if (!musicAudioSource.isPlaying) return;
        StartCoroutine(nameof(FadeMusic));
    }

    private IEnumerator FadeMusic()
    {
        for (float vol = GameManager.Instance.gameOptions.masterVolume; vol >= -1f; vol -= Time.deltaTime / 4)
        {
            musicAudioSource.volume = vol * GameManager.Instance.gameOptions.musicVolume;
            if (vol <= 0) break;
            yield return null;
        }
        
        musicAudioSource.Stop();
    }

    public void PlaySoundScape(AudioClip soundScape)
    {
        if (!ambienceAudioSource) ambienceAudioSource = gameObject.AddComponent<AudioSource>();
        if(ambienceAudioSource.isPlaying) ambienceAudioSource.Stop();
        ambienceAudioSource.volume = GameManager.Instance.gameOptions.masterVolume;
        ambienceAudioSource.clip = soundScape;
        ambienceAudioSource.loop = true;
        ambienceAudioSource.Play();
    }

    public void UpdateVolume()
    {
        var volume = GameManager.Instance.gameOptions.masterVolume;
        
        foreach (var audioSource in audioSources)
        {
            if (!audioSource) continue;
            audioSource.volume = volume;
        }

        foreach (var audioSource in UnsafeAudioSources)
        {
            if (!audioSource) continue;
            audioSource.volume = volume;
        }

        if(musicAudioSource) musicAudioSource.volume = volume * GameManager.Instance.gameOptions.musicVolume;
        if(ambienceAudioSource) ambienceAudioSource.volume = volume;
    }
    
    public void PlaySound(AudioClip audioClip)
    {
        var audioSource = CreateAudioSource();
        audioSource.PlayOneShot(audioClip, GameManager.Instance.gameOptions.masterVolume);
    }

    private void DestroyAudioSources()
    {
        foreach (var audioSource in audioSources)
        {
            if (audioSource == null) continue;
            if(audioSource.isPlaying) continue;
            
            Destroy(audioSource);
        }
    }

    private AudioSource CreateAudioSource()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] == null)
            {
                audioSources[i] = gameObject.AddComponent<AudioSource>();
                return audioSources[i];
            }
        }

        return null;
    }

    public void PlaySoundUnsafe(AudioClip audioClip, int index, bool forcePlay = false)
    {
        if (UnsafeAudioSources[index] == null) return;
        if (UnsafeAudioSources[index].isPlaying && !forcePlay) return;
        
        UnsafeAudioSources[index].clip = audioClip;
        UnsafeAudioSources[index].volume = GameManager.Instance.gameOptions.masterVolume;
        UnsafeAudioSources[index].Play();
        UnsafeAudioSourcesTimer[index] = Time.time;
    }

    public void StopSoundUnsafe(int index)
    {
        if (UnsafeAudioSources[index] == null) return;
        if (!UnsafeAudioSources[index].isPlaying) return;

        UnsafeAudioSources[index].Stop();
    }

    public void PlaySoundUnsafe(AudioClip audioClip, int index, float delay)
    {
        if (UnsafeAudioSources[index] == null) return;
        if (UnsafeAudioSources[index].isPlaying && Time.time - UnsafeAudioSourcesTimer[index] < delay) return;
        
        UnsafeAudioSources[index].PlayOneShot(audioClip, GameManager.Instance.gameOptions.masterVolume);
        UnsafeAudioSourcesTimer[index] = Time.time;
    }
    
    public int CreateUnsafeAudioSource()
    {
        for (int i = 0; i < UnsafeAudioSources.Length; i++)
        {
            if (UnsafeAudioSources[i] == null)
            {
                UnsafeAudioSources[i] = gameObject.AddComponent<AudioSource>();
                UnsafeAudioSourcesTimer[i] = Time.time;
                return i;
            }
        }

        return -1;
    }
    
    public void DestroyUnsafeAudioSource(int index)
    {
        Destroy(UnsafeAudioSources[index]);
    }
}
