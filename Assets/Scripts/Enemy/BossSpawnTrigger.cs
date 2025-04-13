using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BossSpawnTrigger : MonoBehaviour
{
    // Attributes
    public GameObject BossEnemy;
    public Transform BossSpawnPoint;
    public GameObject DoorLeft;
    public GameObject DoorRight;
    public float doorMoveSpeed = 12f;

    // Track Boss
    private bool hasSpawned = false;

    private Vector3 leftDoorTargetPos;
    private Vector3 rightDoorTargetPos;
    private bool moveDoors = false;

    private Camera maincam;
    private CinemachineCamera nearCinemachineCamera;
    private CinemachineCamera farCinemachineCamera;

    void Start()
    {
        leftDoorTargetPos = DoorLeft.transform.position;
        rightDoorTargetPos = DoorRight.transform.position;
        maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        nearCinemachineCamera = GameObject.FindGameObjectWithTag("NearCinemachineCamera").GetComponent<CinemachineCamera>();
        farCinemachineCamera = GameObject.FindGameObjectWithTag("FarCinemachineCamera").GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (moveDoors)
        {
            DoorLeft.transform.position = Vector3.MoveTowards(
                DoorLeft.transform.position, leftDoorTargetPos, doorMoveSpeed * Time.deltaTime);

            DoorRight.transform.position = Vector3.MoveTowards(
                DoorRight.transform.position, rightDoorTargetPos, doorMoveSpeed * Time.deltaTime);
        }
    }

    // Behaviour
    void OnTriggerEnter2D(Collider2D other)
    {
        // Spawn boss if player enters hitbox
        if (other.CompareTag("Player") && !hasSpawned)
        {
            nearCinemachineCamera.Follow = BossSpawnPoint;
            

            nearCinemachineCamera.Priority = 0;
            Instantiate(BossEnemy, BossSpawnPoint.position, Quaternion.identity);
            hasSpawned = true;
            leftDoorTargetPos = DoorLeft.transform.position + new Vector3(0f, -3f, 0f);
            rightDoorTargetPos = DoorRight.transform.position + new Vector3(0f, -3f, 0f);
            moveDoors = true;
            StartCoroutine(WaitForCamTransition(4f)); 
        }
    }

    private IEnumerator WaitForCamTransition(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        nearCinemachineCamera.Priority = -2;
    }

    public void OnBossDefeated()
    {
        leftDoorTargetPos = DoorLeft.transform.position + new Vector3(0f, 3f, 0f);
        rightDoorTargetPos = DoorRight.transform.position + new Vector3(0f, 3f, 0f);
    }
}
