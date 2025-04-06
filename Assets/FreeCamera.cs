using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float upDown = 0;

        if (Input.GetKey(KeyCode.W)) upDown = 1;
        else if (Input.GetKey(KeyCode.S)) upDown = -1;

        Vector3 move = (transform.forward * v + transform.right * h + transform.up * upDown).normalized;
        transform.position += move * moveSpeed * Time.deltaTime;

        // Look around
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            transform.rotation *= Quaternion.Euler(-mouseY, mouseX, 0);
        }
    }
}
