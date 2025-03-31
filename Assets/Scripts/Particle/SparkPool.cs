using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SparkPool : MonoBehaviour
{
    public static SparkPool Instance { get; private set; }

    public GameObject sparkPrefab; // Prefab with Light2D component
    private Queue<GameObject> pool = new Queue<GameObject>();
    // private int count = 0;
    GameObject SparkPoolParent;
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // SparkPoolParent = new GameObject("SparkPoolParent");
        // PreWarmPool(100);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the pool when a new scene is loaded
        foreach (var spark in pool)
        {
            Destroy(spark);
        }
        pool.Clear();
        SparkPoolParent = new GameObject("SparkPoolParent");
        PreWarmPool(100);
    }

    // Get a Light2D object from the pool
    public GameObject GetSpark()
    {
        if (pool.Count > 0)
        {
            GameObject spark = pool.Dequeue();
            spark.SetActive(true);
            return spark;
        }
        else
        {
            // Instantiate a new Light2D object if the pool is empty
            // count++;
            return Instantiate(sparkPrefab, SparkPoolParent.transform);
        }
    }

    public int GetPoolCount()
    {
        return pool.Count;
    }

    // Return a Light2D object to the pool
    public void ReturnSpark(GameObject spark)
    {
        spark.SetActive(false);
        pool.Enqueue(spark);
    }

    // Pre-warm the pool with a specified number of lights
    public void PreWarmPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject spark = Instantiate(sparkPrefab, SparkPoolParent.transform);
            spark.SetActive(false);
            pool.Enqueue(spark);
        }
    }

    private void OnDisable()
    {
        // Clear the pool when the object is disabled
        foreach (var spark in pool)
        {
            Destroy(spark);
        }
        pool.Clear();
        // Debug.Log("Count: " + count);
    }
}