using UnityEngine;

public class DroneTurnaround : MonoBehaviour
{
    // Attributes
    public bool AllowMoveThroughLeft = false;
    public bool AllowMoveThroughRight = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Turn enemy around if enemy is drone
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && enemy is Enemy_Drone)
            {
                Enemy_Drone enemyDrone = (Enemy_Drone) enemy;
                // Don't turn around if allowing move through
                if (
                    (AllowMoveThroughLeft && enemyDrone.GetDirection() == Vector3.left)
                    ||
                    (AllowMoveThroughRight && enemyDrone.GetDirection() == Vector3.right)
                ) {return;}
                
                // Otherwise, turn around drone
                else {enemyDrone.ChangeDirection(true);}
            }
        }
    }
}
