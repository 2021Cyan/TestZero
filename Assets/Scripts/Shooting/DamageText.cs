using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float floatSpeed = 5.0f;
    public float lifetime = 0.2f;  
    private TextMeshPro textMesh;
    private Vector3 randomDirection;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        randomDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1.2f, 0).normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += randomDirection * floatSpeed * Time.deltaTime;
    }

    public void SetDamageText(int damage)
    {
        textMesh.text = damage.ToString();
    }
}
