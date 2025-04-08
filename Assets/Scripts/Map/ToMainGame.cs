using UnityEngine;

public class ToMainGame : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = PlayerController.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerController != null && other.CompareTag("Player"))
        {
           playerController.Restart();
        }
    }
}
