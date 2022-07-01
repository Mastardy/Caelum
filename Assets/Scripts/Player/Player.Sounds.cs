using UnityEngine;

public partial class Player
{
    [SerializeField] private GameObject audioSource;
    private float lastFootStep;
    private int audioSrc;

    private void Awake()
    {
        audioSrc = AudioManager.Instance.CreateUnsafeAudioSource();
        AudioManager.Instance.DestroyUnsafeAudioSource(audioSrc);
        AudioManager.Instance.UnsafeAudioSources[audioSrc] = Instantiate(audioSource, transform).GetComponent<AudioSource>();
    }

    private int lastIndex;
    private int index;

    private void PlayFootstepSounds()
    {
        if (!isGrounded) return;
        if (IsTethered()) return;
        if (horizontalVelocity.magnitude < 1) return;

        if (Time.time - lastFootStep < 0.5f) return;
        
        Collider[] results = new Collider[1];

        Physics.OverlapSphereNonAlloc(transform.position, 2, results, groundMask);

        if(results.Length == 0) return;
        
        if (horizontalVelocity.magnitude <= speed)
        {
            var lightWalking = Resources.LoadAll<AudioClip>($"Sounds/Player/{results[0].tag}/Steps");

            if (lightWalking.Length == 0) return;
            if (lightWalking.Length > 1)
            {
                do
                {
                    index = Random.Range(0, lightWalking.Length);
                } while (lastIndex == index);
            }
            else index = 0;

            AudioManager.Instance.PlaySoundUnsafe(lightWalking[index], audioSrc);

            lastIndex = index;

            lastFootStep = Time.time;
            
            return;
        }
        
        var lightRunning = Resources.LoadAll<AudioClip>($"Sounds/Player/{results[0].tag}/Run");
        
        if (lightRunning.Length == 0) return;
        
        do { index = Random.Range(0, lightRunning.Length); } while (lastIndex == index);
        
        AudioManager.Instance.PlaySoundUnsafe(lightRunning[index], audioSrc, lightRunning[index].length / 2f);
        
        lastFootStep = Time.time - 0.2f;
    }
}
