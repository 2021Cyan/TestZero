using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GunCreate : MonoBehaviour
{
    public GameObject gunPrefab;
    public Transform[] spawnPoints;
    public float interactionRange = 2.0f;
    public int gunCost = 160;

    private Transform player;
    private PlayerController playerController;
    private AudioManager _audio;
    private InputManager _input;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();

    private static int totalGunsCreated = 0;

    private Animator animator; 
    private bool isPlayerNearby = false;
    private bool isGeneratingGuns = false;
    private double holdTime = 0f;
    private bool isRecycling = false;
    private double recycleHoldTime = 0f;
    private double threshold = 0.6f;


    void Start()
    {
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // float distance = Vector2.Distance(transform.position, player.position);
        // isPlayerNearby = distance <= interactionRange;

        //TODO: use collider to check if player is nearby
        // if (isPlayerNearby)
        // {
        //     animator.SetBool("isNearby", true);
        // }
        // else
        // {
        //     if(occupiedSpawnPoints.Count == 0)
        //     {
        //         animator.SetBool("isNearby", false);
        //     }
        // }

        // if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        if (isPlayerNearby && _input.FInput)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("opened"))
            {
                StartCoroutine(GenerateMultipleGuns());
            }
        }

        if (isPlayerNearby && _input.GInput)
        {
            recycleHoldTime = Time.fixedUnscaledTime - _input.GetGPressTime();
            if (!isRecycling && recycleHoldTime >= threshold)
            {
                StartCoroutine(Recycle());
            }
        }
        // //TODO: change with InputManager
        // if (Input.GetKeyUp(KeyCode.G))
        // {
        //     recycleHoldTime = 0f;
        //     isRecycling = false;
        // }
    }

    IEnumerator Recycle()
    {
        if (isRecycling) yield break;
        isRecycling = true;

        List<Transform> toRemove = new List<Transform>();
        //TODO: Change this I am testing this for now
        _audio.SetParameterByName("Shop", 3);
        
        foreach (Transform spawnPoint in occupiedSpawnPoints)
        {
            Collider2D[] gunColliders = Physics2D.OverlapCircleAll(spawnPoint.position, 0.1f);
            foreach (Collider2D gunCollider in gunColliders)
            {
                GunScript gunScript = gunCollider.GetComponent<GunScript>();
                if (gunScript != null)
                {
                    _audio.PlayOneShot(_audio.Shop);
                    int refundAmount = 0;
                    switch (gunScript.gunRarity)
                    {
                        case GunScript.Rarity.Common:
                            refundAmount = 40;
                            break;
                        case GunScript.Rarity.Uncommon:
                            refundAmount = 160;
                            break;
                        case GunScript.Rarity.Rare:
                            refundAmount = 320;
                            break;
                        case GunScript.Rarity.Legendary:
                            refundAmount = 640;
                            break;
                    }
                    playerController.AddResource(refundAmount);
                    Destroy(gunCollider.gameObject);
                    toRemove.Add(spawnPoint);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        foreach (Transform remove in toRemove)
        {
            occupiedSpawnPoints.Remove(remove);
        }

        isRecycling = false;
        recycleHoldTime = 0f;
    }

    IEnumerator GenerateMultipleGuns()
    {
        if (isGeneratingGuns) yield break; 
        isGeneratingGuns = true;
        holdTime = _input.GetFPressTime();
        while (_input.GetFPressTime() != 0)
        {
            yield return null;
            if (Time.fixedUnscaledTime - holdTime >= threshold)
            {
                for (int i = 0; i < 10; i++)
                {
                    TryGenerateGun();
                    yield return new WaitForSeconds(0.3f);
                }
                isGeneratingGuns = false;
                yield break;
            }
        }
        if (Time.fixedUnscaledTime - holdTime < threshold)
        {
            TryGenerateGun();
        }
        isGeneratingGuns = false;
    }

    //TODO: Change _audio.PlayOneShot("Shop") as I update Inputmanager
    void TryGenerateGun()
    {
        if (playerController != null && playerController.resource >= gunCost)
        {
            _audio.SetParameterByName("Shop", 2);
            Transform availableSpawn = GetAvailableSpawnPoint();
            if (availableSpawn != null)
            {
                playerController.resource -= gunCost;
                GameObject spawnedGun = Instantiate(gunPrefab, availableSpawn.position, Quaternion.Euler(0,0,90));
                occupiedSpawnPoints.Add(availableSpawn);
                _audio.PlayOneShot(_audio.Shop);

                GunScript gunScript = spawnedGun.GetComponent<GunScript>();
                if (gunScript == null)
                {
                    gunScript = spawnedGun.AddComponent<GunScript>();
                }
                totalGunsCreated++;
                gunScript.AssignRarityWithPity(totalGunsCreated);
                gunScript.DistributePartLevels();
                gunScript.AssignGripType();
                gunScript.SetGunCreateStation(this);
                GunInfoScript infoScript = GetComponentInChildren<GunInfoScript>(true);
                if (infoScript != null)
                {
                    gunScript.SetInfoPanel(infoScript);
                }
                GunInfoRender infoRender = GetComponentInChildren<GunInfoRender>(true);
                if (infoRender != null)
                {
                    gunScript.SetInfoRenderer(infoRender);
                }
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

    public void ReleaseSlot(Vector3 gunPosition)
    {
        Transform releasedSlot = null;

        foreach (Transform spawnPoint in occupiedSpawnPoints)
        {
            if (Vector3.Distance(spawnPoint.position, gunPosition) < 0.1f)
            {
                releasedSlot = spawnPoint;
                break;
            }
        }

        if (releasedSlot != null)
        {
            occupiedSpawnPoints.Remove(releasedSlot); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //TODO: There should be a cooldown for this
            // if(animator.GetBool("isNearby") == false)
            // {
            //     _audio.SetParameterByName("Shop", 1);
            //     _audio.PlayOneShot(_audio.Shop);
            // }
            isPlayerNearby = true;
            animator.SetBool("isNearby", true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if(occupiedSpawnPoints.Count == 0)
            {
                // _audio.SetParameterByName("Shop", 2);
                // _audio.PlayOneShot(_audio.Shop);
                animator.SetBool("isNearby", false);
            }
        }
    }
}
