using UnityEngine;
using System.Collections.Generic;

public class GunCreate : MonoBehaviour
{
    public GameObject gunPrefab;
    public Transform[] spawnPoints;
    public float interactionRange = 2.0f;
    public int gunCost = 160;

    private Transform player;
    private PlayerController playerController;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();

    private static int totalGunsCreated = 0;

    private Animator animator; 
    private bool isPlayerNearby = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasPlayerNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionRange;

        if (isPlayerNearby && !wasPlayerNearby)
        {
            animator.SetBool("isNearby", true);
        }
        else if (!isPlayerNearby && wasPlayerNearby)
        {
            animator.SetBool("isNearby", false);
        }

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
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
                GameObject spawnedGun = Instantiate(gunPrefab, availableSpawn.position, Quaternion.Euler(0,0,90));
                occupiedSpawnPoints.Add(availableSpawn);

                GunScript gunScript = spawnedGun.GetComponent<GunScript>();
                if (gunScript == null)
                {
                    gunScript = spawnedGun.AddComponent<GunScript>();
                }
                totalGunsCreated++;
                gunScript.AssignRarityWithPity(totalGunsCreated);
                gunScript.DistributePartLevels();
                gunScript.AssignGripType();
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
