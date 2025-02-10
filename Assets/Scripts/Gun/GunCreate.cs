using UnityEngine;
using System.Collections.Generic;

public class GunCreate : MonoBehaviour
{
    public GameObject gunPrefab;
    // Array of spawn points
    public Transform[] spawnPoints;
    // Distance for player to interact
    public float interactionRange = 2.0f;
    // Cost of generating a gun
    public int gunCost = 160;

    // Reference to the player
    private Transform player;
    PlayerController playerController;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= interactionRange && Input.GetKeyDown(KeyCode.F))
        {
            TryGenerateGun();
        }
    }

    void TryGenerateGun()
    {
        if (playerController != null && playerController.resource >= gunCost)
        {
            Transform availableSpawn = GetAvailableSpawnPoint();
            if (availableSpawn != null)
            {
                playerController.resource -= gunCost; 
                GameObject spawnedGun = Instantiate(gunPrefab, availableSpawn.position, Quaternion.identity);
                occupiedSpawnPoints.Add(availableSpawn);
            }
        }
    }

    Transform GetAvailableSpawnPoint()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (!occupiedSpawnPoints.Contains(spawnPoint))
            {
                return spawnPoint;
            }
        }
        return null; 
    }
}
