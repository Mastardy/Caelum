using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    private Player player;
    public int cameraShakeAmount = 10;
    public int damage = 25;
    public ParticleSystem[] particleSystems;
    public Vector2 burstDelay = new Vector2(10f, 20f);

    private void Start()
    {
        Invoke("StartPlaying", Random.Range(burstDelay.x, burstDelay.y));
    }

    private void StartPlaying()
    {
        foreach (var ps in particleSystems) ps.Play();
        if (player)
        {
            player.TakeDamageServerRpc(damage);
            player.cameraShake.PlayShake(cameraShakeAmount);
        }

        Invoke("StartPlaying", Random.Range(burstDelay.x, burstDelay.y));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Player>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.cameraShake.StopShake();
            player = null;
        }
    }

}
