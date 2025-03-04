using UnityEngine;

public class PodScript : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerController;

    // Pod movement
    public float followSpeed = 8f;
    public Vector3 offset = new Vector3(-1f, 2.0f, 0f);
    public float swayAmount = 0.2f;
    public float swaySpeed = 3f;

    // Pod behaviour
    public int weapontype;


    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            weapontype = 0;
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = playerController.GetAimPos().position + offset;
        float swayOffset = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        targetPosition.y += swayOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
    }

    private void Shoot()
    {

    }

    private void ShootMissile()
    {

    }

    private void Heal()
    {

    }

    private void Shield()
    {

    }
}

