using UnityEngine;
using TMPro;

public class ResourceText : MonoBehaviour
{
    public float floatSpeed = 3.0f;
    public float lifetime = 0.5f;  
    private TextMeshPro textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime; 
    }

    public void SetResourceText(int amount)
    {
        textMesh.text = amount.ToString();
    }
}
