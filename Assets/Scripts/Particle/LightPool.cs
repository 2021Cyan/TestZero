using System.Collections.Generic;
using UnityEngine;

public class LightPool : MonoBehaviour
{
    public static LightPool Instance { get; private set; }

    public GameObject lightPrefab; // Prefab with Light2D component
    private Queue<GameObject> pool = new Queue<GameObject>();
    private Transform temp;
    // private int count = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GameObject tempObject = GameObject.Find("temp");
        if (tempObject == null)
        {
            tempObject = new GameObject("temp");
        }
        temp = tempObject.transform;
        PreWarmPool(30);
    }

    // Get a Light2D object from the pool
    public GameObject GetLight()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            // Instantiate a new Light2D object if the pool is empty
            // count++;
            return Instantiate(lightPrefab);
        }
    }

    public int GetPoolCount()
    {
        return pool.Count;
    }

    // Return a Light2D object to the pool
    public void ReturnLight(GameObject light)
    {
        light.SetActive(false);
        pool.Enqueue(light);
    }

    // Pre-warm the pool with a specified number of lights
    public void PreWarmPool(int count)
    {
        // Transform temp = ;
        for (int i = 0; i < count; i++)
        {
            GameObject light = Instantiate(lightPrefab);
            light.SetActive(false);
            pool.Enqueue(light);
            light.transform.SetParent(temp);
        }
    }

    private void OnDisable()
    {
        // Clear the pool when the object is disabled
        foreach (var light in pool)
        {
            Destroy(light);
        }
        pool.Clear();
        // Debug.Log("Count: " + count);
    }
}