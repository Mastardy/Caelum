using UnityEngine;

public partial class Player
{
    [SerializeField] private GameObject audioSource;
    private float lastFootStep;
    private GameObject audioSrc;

    private void Awake()
    {
        audioSrc = Instantiate(audioSource, transform);
    }

    private int lastIndex;
    private int index;

    private void PlayFootstepSounds()
    {
        if (!isGrounded) return;
        if (horizontalVelocity.magnitude < 1) return;

        if (Time.time - lastFootStep < 0.5f) return;
        
        Collider[] results = new Collider[1];

        Physics.OverlapSphereNonAlloc(transform.position, 2, results, groundMask);

        if (horizontalVelocity.magnitude <= speed)
        {
            var lightWalking = Resources.LoadAll<AudioClip>($"Sounds/Player/{results[0].tag}/Steps/Light");

            if (lightWalking.Length == 0) return;
            if (lightWalking.Length > 1)
            {
                do
                {
                    index = Random.Range(0, lightWalking.Length);
                } while (lastIndex == index);
            }
            else index = 0;
                
            audioSrc.GetComponent<AudioSource>().PlayOneShot(lightWalking[index]);

            lastIndex = index;

            lastFootStep = Time.time;
            
            return;
        }
        
        var lightRunning = Resources.LoadAll<AudioClip>($"Sounds/Player/{results[0].tag}/Run/Light");
        
        if (lightRunning.Length == 0) return;
        
        do { index = Random.Range(0, lightRunning.Length); } while (lastIndex == index);
        
        audioSrc.GetComponent<AudioSource>().PlayOneShot(lightRunning[index]);
        
        lastFootStep = Time.time - 0.2f;
    }

    private void PlayToolSwing(string itemTag)
    {
        var toolSwing = Resources.LoadAll<AudioClip>($"Sounds/Player/Tools/{itemTag}/Swing/");
        if (toolSwing.Length == 0)
        {
            Debug.LogWarning(itemTag + " doesn't have Swing Sounds!");
            return;
        }
        audioSrc.GetComponent<AudioSource>().PlayOneShot(toolSwing[Random.Range(0, toolSwing.Length)]);
    }
}
