using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource[] audioSources = new AudioSource[128];
    public AudioSource[] unsafeAudioSources { get; } = new AudioSource[32];
    public float[] unsafeAudioSourcesTimer { get; } = new float[32];

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InvokeRepeating(nameof(DestroyAudioSources), 5.0f, 5.0f);
    }

    public void UpdateVolume(float volume)
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
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
        if (unsafeAudioSources[index] == null) return;
        if (unsafeAudioSources[index].isPlaying && !forcePlay) return;
        
        unsafeAudioSources[index].PlayOneShot(audioClip, GameManager.Instance.gameOptions.masterVolume);
        unsafeAudioSourcesTimer[index] = Time.time;
    }

    public void PlaySoundUnsafe(AudioClip audioClip, int index, float delay)
    {
        if (unsafeAudioSources[index] == null) return;
        if (unsafeAudioSources[index].isPlaying && Time.time - unsafeAudioSourcesTimer[index] < delay) return;
        
        unsafeAudioSources[index].PlayOneShot(audioClip, GameManager.Instance.gameOptions.masterVolume);
        unsafeAudioSourcesTimer[index] = Time.time;
    }
    
    public int CreateUnsafeAudioSource()
    {
        for (int i = 0; i < unsafeAudioSources.Length; i++)
        {
            if (unsafeAudioSources[i] == null)
            {
                unsafeAudioSources[i] = gameObject.AddComponent<AudioSource>();
                unsafeAudioSourcesTimer[i] = Time.time;
                return i;
            }
        }

        return -1;
    }
    
    public void DestroyUnsafeAudioSource(int index)
    {
        Destroy(unsafeAudioSources[index]);
    }
}
