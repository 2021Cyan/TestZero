using UnityEngine;

public class RaftSectionBlock : MonoBehaviour
{
    // Public attributes
    public Transform EntryPoint;
    public Transform ExitPoint;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (EntryPoint != null)
        {
            Gizmos.DrawSphere(EntryPoint.position, 0.5f);
        }
        if (ExitPoint != null)
        {
            Gizmos.DrawSphere(ExitPoint.position, 0.5f);
        }
    }
}