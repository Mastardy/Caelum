using UnityEngine;

public partial class Player
{
    [Header("Spawning")]
    [SerializeField] private Vector3 spawnPosition;

    private void SpawnPlayer()
    {
        transform.position = spawnPosition;
    }
}
