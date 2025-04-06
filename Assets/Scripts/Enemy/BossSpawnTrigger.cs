using UnityEngine;

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

    void Start()
    {
        leftDoorTargetPos = DoorLeft.transform.position;
        rightDoorTargetPos = DoorRight.transform.position;
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
            Instantiate(BossEnemy, BossSpawnPoint.position, Quaternion.identity);
            hasSpawned = true;
            leftDoorTargetPos = DoorLeft.transform.position + new Vector3(0f, -3f, 0f);
            rightDoorTargetPos = DoorRight.transform.position + new Vector3(0f, -3f, 0f);
            moveDoors = true;
        }
    }

    public void OnBossDefeated()
    {
        leftDoorTargetPos = DoorLeft.transform.position + new Vector3(0f, 3f, 0f);
        rightDoorTargetPos = DoorRight.transform.position + new Vector3(0f, 3f, 0f);
    }
}
