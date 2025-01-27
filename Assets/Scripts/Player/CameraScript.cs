using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Transform of the player
    public Transform player;
    // How smooth the camera will move 
    public float smoothSpeed = 0.125f;
    // Offset of the camera from the player
    public Vector3 offset;      

    void LateUpdate()
    {
        // If the player is not Null, move the camera
        if (player != null)
        {
            // Desired position of the camera
            Vector3 desiredPosition = player.position + offset;
            // Smoothed postion of the camera
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            // Move the camera
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
        }
    }
}