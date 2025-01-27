using UnityEngine;

public class ReticleController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Gets the current position of the player mouse 
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Changes the position into mousePos
        transform.position = mousePos;
    }
}
