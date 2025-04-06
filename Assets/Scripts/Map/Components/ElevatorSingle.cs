using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ElevatorSingle : MonoBehaviour
{
    public GameObject wall;
    public Transform TopPoint;
    public Transform BottomPoint;
    public float Velocity = 2f;

    private bool hasMoved = false;
    private Vector3 prevPosition;
    private Vector3 movement;
    private HashSet<Transform> entitiesOnPlatform = new HashSet<Transform>();

    private void Start()
    {
        if(wall != null)
        {
            wall.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasMoved)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            entitiesOnPlatform.Add(other.transform);

            if (other.CompareTag("Player"))
            {
                StartCoroutine(StartElevatorAfterDelay(0.5f));
            }
        }
    }

    private IEnumerator StartElevatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(MoveElevator());
    }

    private IEnumerator MoveElevator()
    {
        hasMoved = true;
        Vector3 target = TopPoint.position;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, target, Velocity * Time.deltaTime);
            movement = transform.position - prevPosition;

            foreach (Transform tf in entitiesOnPlatform)
            {
                tf.position += movement;
            }

            yield return null;
        }
        movement = Vector3.zero;
    }

}