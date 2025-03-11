using System.Collections;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [Header("Ghost Trail Settings")]
    public GameObject ghostPrefab;
    public float spawnInterval = 0.05f;  
    public float ghostLifetime = 0.7f; 
    private int ghostIndex = 0;
    private int maxGhosts = 20;

    private Color[] ghostColors = new Color[]
    {
        new Color(0.6f, 1.0f, 0.4f),
        new Color(0.55f, 0.95f, 0.5f),
        new Color(0.5f, 0.9f, 0.6f),
        new Color(0.45f, 0.85f, 0.65f),
        new Color(0.4f, 0.8f, 0.7f),
        new Color(0.35f, 0.75f, 0.75f),
        new Color(0.3f, 0.7f, 0.85f),
        new Color(0.28f, 0.68f, 0.9f),
        new Color(0.25f, 0.65f, 0.95f),
        new Color(0.2f, 0.6f, 1.0f),
        new Color(0.18f, 0.55f, 1.0f),
        new Color(0.16f, 0.5f, 1.0f),
        new Color(0.3f, 0.4f, 1.0f),
        new Color(0.5f, 0.3f, 1.0f),
        new Color(0.6f, 0.25f, 1.0f),
        new Color(0.75f, 0.2f, 0.9f),
        new Color(0.85f, 0.15f, 0.7f),
        new Color(0.9f, 0.1f, 0.5f),
        new Color(0.95f, 0.05f, 0.3f),
        new Color(1.0f, 0.2f, 0.2f) 
    };

    void Start()
    {
        StartCoroutine(SpawnGhosts());
    }

    IEnumerator SpawnGhosts()
    {
        while (true)
        {
            SpawnGhost();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnGhost()
    {
        int colorIndex = ghostIndex % maxGhosts;
        Color ghostColor = ghostColors[maxGhosts - 1 - colorIndex];
        ghostIndex++;

        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation, transform.parent);
        ghost.transform.localScale = transform.localScale;

        Animator ghostAnim = ghost.GetComponent<Animator>();
        if (ghostAnim != null)
        {
            ghostAnim.enabled = false;
        }

        SpriteRenderer[] spriteRenderers = ghost.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in spriteRenderers)
        {
            sr.color = ghostColor;
        }

        Ghost ghostScript = ghost.GetComponent<Ghost>();
        if (ghostScript != null)
        {
            ghostScript.StartFade(ghostLifetime);
        }
    }
}